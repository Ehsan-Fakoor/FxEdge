using FxEdge.Business.Abstractions;
using FxEdge.Business.Catalog;
using FxEdge.Business.Entities;
using FxEdge.Business.ValueObjects;
using FxEdge.Contracts.Dtos.Entries;
using FxEdge.Contracts.Services;

namespace FxEdge.Business.Services;

/// <inheritdoc cref="IEntryService"/>
public sealed class EntryService : IEntryService
{
    private readonly IEntryRepository _entryRepository;
    private readonly IFormulaFeatureCalculator _formulaCalculator;
    private readonly PointInTimeResolver _resolver;
    private readonly FeatureNormalizer _normalizer;

    public EntryService(IEntryRepository entryRepository, IObservationRepository observationRepository, IFormulaFeatureCalculator formulaCalculator)
    {
        _entryRepository = entryRepository;
        _formulaCalculator = formulaCalculator;
        _resolver = new PointInTimeResolver(observationRepository);
        _normalizer = new FeatureNormalizer(observationRepository);
    }

    public async Task<EntryDto> CreateAsync(CreateEntryRequest request, CancellationToken ct = default)
    {
        // Throws FxEdgeValidationException if this isn't one of the 28 supported pairs.
        var pair = new CurrencyPair(request.BaseCurrency, request.QuoteCurrency);

        var fundamentalSnapshot = await BuildFundamentalSnapshotAsync(pair, request.EntryAtUtc, ct);
        var technicalSnapshot = BuildTechnicalSnapshot(request.Technical);

        // Z(Feature) in the formula spec = the Difference already resolved above for
        // that feature - no further I/O needed to compute the four formulas.
        var differencesByFeature = fundamentalSnapshot.ToDictionary(s => s.Feature, s => s.Difference);

        var formula1W = _formulaCalculator.Calculate(differencesByFeature, FormulaPeriod.OneWeek);
        var formula1M = _formulaCalculator.Calculate(differencesByFeature, FormulaPeriod.OneMonth);
        var formula3M = _formulaCalculator.Calculate(differencesByFeature, FormulaPeriod.ThreeMonths);
        var formula1Y = _formulaCalculator.Calculate(differencesByFeature, FormulaPeriod.OneYear);

        var entry = Entry.Create(
            request.BaseCurrency,
            request.QuoteCurrency,
            request.EntryAtUtc,
            request.EntryPrice,
            request.Direction,
            fundamentalSnapshot,
            technicalSnapshot,
            formula1W,
            formula1M,
            formula3M,
            formula1Y);

        await _entryRepository.AddAsync(entry, ct);
        await _entryRepository.SaveChangesAsync(ct);

        return ToDto(entry);
    }

    public async Task<EntryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entry = await _entryRepository.GetByIdAsync(id, ct);
        return entry is null ? null : ToDto(entry);
    }

    public async Task<IReadOnlyList<EntrySummaryDto>> QueryAsync(EntryQuery query, CancellationToken ct = default)
    {
        var results = await _entryRepository.QueryAsync(
            query.BaseCurrency, query.QuoteCurrency, query.FromUtc, query.ToUtc, query.Direction, ct);

        return results.Select(ToSummaryDto).ToList();
    }

    /// <summary>
    /// For every one of the 16 fundamental features, resolves the Base and Quote
    /// point-in-time state strictly as of EntryAtUtc (never later), normalizes each
    /// side's Value against its own point-in-time history (FeatureNormalizer), and
    /// freezes the result. This is what makes the Entry's fundamental picture
    /// leakage-free and immutable to future corrections.
    /// </summary>
    private async Task<List<EntryFundamentalFeatureSnapshot>> BuildFundamentalSnapshotAsync(CurrencyPair pair, DateTime entryAtUtc, CancellationToken ct)
    {
        var snapshot = new List<EntryFundamentalFeatureSnapshot>(FundamentalFeatureCatalog.All.Count);

        foreach (var definition in FundamentalFeatureCatalog.All)
        {
            var baseFeature = await _resolver.ResolveAsync(pair.Base, definition.Feature, entryAtUtc, ct);
            var quoteFeature = await _resolver.ResolveAsync(pair.Quote, definition.Feature, entryAtUtc, ct);

            var baseNormalized = await _normalizer.ResolveNormalizedValueAsync(pair.Base, definition.Feature, entryAtUtc, ct);
            var quoteNormalized = await _normalizer.ResolveNormalizedValueAsync(pair.Quote, definition.Feature, entryAtUtc, ct);

            var differential = DifferentialCalculator.Calculate(baseFeature, quoteFeature, baseNormalized, quoteNormalized);

            snapshot.Add(new EntryFundamentalFeatureSnapshot(
                definition.Feature, definition.Polarity, baseFeature, quoteFeature,
                baseNormalized, quoteNormalized, differential.DifferentialValue));
        }

        return snapshot;
    }

    /// <summary>
    /// Builds a full 7-entry technical snapshot (fixed order), filling in null for any
    /// feature the caller did not supply a value for.
    /// </summary>
    private static List<EntryTechnicalFeatureSnapshot> BuildTechnicalSnapshot(IReadOnlyList<TechnicalFeatureValueDto> input)
    {
        var byFeature = input
            .GroupBy(x => x.Feature)
            .ToDictionary(g => g.Key, g => g.Last().Value);

        var snapshot = new List<EntryTechnicalFeatureSnapshot>(TechnicalFeatureCatalog.All.Count);
        foreach (var definition in TechnicalFeatureCatalog.All)
        {
            byFeature.TryGetValue(definition.Feature, out var value);
            snapshot.Add(new EntryTechnicalFeatureSnapshot(definition.Feature, value));
        }

        return snapshot;
    }

    private static EntryDto ToDto(Entry entry) => new(
        entry.Id,
        entry.BaseCurrency,
        entry.QuoteCurrency,
        entry.Pair.Symbol,
        entry.EntryAtUtc,
        entry.EntryPrice,
        entry.Direction,
        entry.FundamentalSnapshot.OrderBy(s => s.Feature).Select(ToFundamentalDto).ToList(),
        entry.TechnicalSnapshot.OrderBy(s => s.Feature).Select(ToTechnicalDto).ToList(),
        new EntryFormulaFeaturesDto(entry.Formula1W, entry.Formula1M, entry.Formula3M, entry.Formula1Y),
        entry.AtrValue);

    private static EntryFundamentalFeatureSnapshotDto ToFundamentalDto(EntryFundamentalFeatureSnapshot s) => new(
        s.Feature,
        s.Polarity,
        s.BaseValue,
        s.BasePreviousValue,
        s.BaseActualVsPrevious,
        s.BaseAnnouncementAtUtc,
        s.BaseNormalizedValue,
        s.QuoteValue,
        s.QuotePreviousValue,
        s.QuoteActualVsPrevious,
        s.QuoteAnnouncementAtUtc,
        s.QuoteNormalizedValue,
        s.Difference);

    private static TechnicalFeatureValueDto ToTechnicalDto(EntryTechnicalFeatureSnapshot s) => new(s.Feature, s.Value);

    private static EntrySummaryDto ToSummaryDto(Entry entry) => new(
        entry.Id, entry.BaseCurrency, entry.QuoteCurrency, entry.Pair.Symbol, entry.EntryAtUtc, entry.EntryPrice, entry.Direction);
}
