using FxEdge.Business.Abstractions;
using FxEdge.Business.Catalog;
using FxEdge.Business.Common;
using FxEdge.Business.ValueObjects;
using FxEdge.Contracts.Dtos.Dataset;
using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Services;

namespace FxEdge.Business.Services;

/// <inheritdoc cref="IDatasetService"/>
public sealed class DatasetService : IDatasetService
{
    private readonly PointInTimeResolver _resolver;
    private readonly FeatureNormalizer _normalizer;

    public DatasetService(IObservationRepository repository)
    {
        _resolver = new PointInTimeResolver(repository);
        _normalizer = new FeatureNormalizer(repository);
    }

    public async Task<CurrencyFundamentalSnapshotDto> GetCurrencySnapshotAsync(Currency currency, DateTime asOfUtc, CancellationToken ct = default)
    {
        var asOfNormalized = DateTimeUtc.Normalize(asOfUtc);

        var features = new List<FeatureSnapshotDto>(FundamentalFeatureCatalog.All.Count);
        foreach (var definition in FundamentalFeatureCatalog.All)
        {
            features.Add(await _resolver.ResolveAsync(currency, definition.Feature, asOfNormalized, ct));
        }

        return new CurrencyFundamentalSnapshotDto(currency, asOfNormalized, features);
    }

    public async Task<FundamentalDatasetRowDto> GetDatasetRowAsync(Currency baseCurrency, Currency quoteCurrency, DateTime asOfUtc, CancellationToken ct = default)
    {
        // Throws FxEdgeValidationException if this isn't one of the 28 supported pairs.
        var pair = new CurrencyPair(baseCurrency, quoteCurrency);
        var asOfNormalized = DateTimeUtc.Normalize(asOfUtc);

        var baseSnapshot = await GetCurrencySnapshotAsync(pair.Base, asOfNormalized, ct);
        var quoteSnapshot = await GetCurrencySnapshotAsync(pair.Quote, asOfNormalized, ct);

        // Both snapshots iterate FundamentalFeatureCatalog.All in the same fixed order,
        // so positional pairing is safe here.
        var differentials = new List<PairFeatureDifferentialDto>(baseSnapshot.Features.Count);
        for (var i = 0; i < baseSnapshot.Features.Count; i++)
        {
            var baseFeature = baseSnapshot.Features[i];
            var quoteFeature = quoteSnapshot.Features[i];

            var baseNormalized = await _normalizer.ResolveNormalizedValueAsync(pair.Base, baseFeature.Feature, asOfNormalized, ct);
            var quoteNormalized = await _normalizer.ResolveNormalizedValueAsync(pair.Quote, quoteFeature.Feature, asOfNormalized, ct);

            differentials.Add(DifferentialCalculator.Calculate(baseFeature, quoteFeature, baseNormalized, quoteNormalized));
        }

        return new FundamentalDatasetRowDto(
            pair.Symbol,
            pair.Base,
            pair.Quote,
            asOfNormalized,
            baseSnapshot,
            quoteSnapshot,
            differentials);
    }

    public async Task<IReadOnlyList<FundamentalDatasetRowDto>> GetAllPairsDatasetRowsAsync(DateTime asOfUtc, CancellationToken ct = default)
    {
        var rows = new List<FundamentalDatasetRowDto>(CurrencyPair.All.Count);
        foreach (var pair in CurrencyPair.All)
        {
            rows.Add(await GetDatasetRowAsync(pair.Base, pair.Quote, asOfUtc, ct));
        }

        return rows;
    }
}
