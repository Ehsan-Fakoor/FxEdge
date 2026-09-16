using FxEdge.Business.Common;
using FxEdge.Business.ValueObjects;
using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;

namespace FxEdge.Business.Entities;

/// <summary>
/// A manually marked chart entry point. Has no relation to any automatic system
/// detection - the user identifies the entry point by looking at the chart.
///
/// Fully immutable once created: there is intentionally no edit/delete in this phase.
/// The frozen FundamentalSnapshot and TechnicalSnapshot children, together with the
/// four Formula features, are supplied atomically at construction time by EntryService
/// (which is the only place that can read observation history / compute technicals -
/// the entity itself performs no I/O).
/// </summary>
public sealed class Entry
{
    public Guid Id { get; private set; }
    public Currency BaseCurrency { get; private set; }
    public Currency QuoteCurrency { get; private set; }
    public DateTime EntryAtUtc { get; private set; }
    public decimal EntryPrice { get; private set; }
    public Direction Direction { get; private set; }

    public decimal? Formula1W { get; private set; }
    public decimal? Formula1M { get; private set; }
    public decimal? Formula3M { get; private set; }
    public decimal? Formula1Y { get; private set; }

    private readonly List<EntryFundamentalFeatureSnapshot> _fundamentalSnapshot = new();
    public IReadOnlyList<EntryFundamentalFeatureSnapshot> FundamentalSnapshot => _fundamentalSnapshot;

    private readonly List<EntryTechnicalFeatureSnapshot> _technicalSnapshot = new();
    public IReadOnlyList<EntryTechnicalFeatureSnapshot> TechnicalSnapshot => _technicalSnapshot;

    /// <summary>The validated Base/Quote pair. Computed, never persisted.</summary>
    public CurrencyPair Pair => new(BaseCurrency, QuoteCurrency);

    /// <summary>
    /// The ATR reading frozen on this Entry's TechnicalSnapshot at entry time - the
    /// single reference every "...Atr"-suffixed quantity elsewhere (EntryResult's
    /// MFE/MAE/ReturnAtHorizonEndAtr, EntryDailyCandle's OpenAtr/HighAtr/LowAtr/CloseAtr,
    /// and a Strategy's EntrySpacingAtr/TakeProfitPerEntryAtr/OverallStopLossAtr once
    /// simulated) must be multiplied against to get an actual price-equivalent value.
    /// Null if ATR was never entered for this Entry (Technical readings are optional).
    /// </summary>
    public decimal? AtrValue => TechnicalSnapshot.SingleOrDefault(t => t.Feature == TechnicalFeature.Atr)?.Value;

    // EF Core materialization constructor.
    private Entry()
    {
    }

    private Entry(Guid id, Currency baseCurrency, Currency quoteCurrency, DateTime entryAtUtc, decimal entryPrice, Direction direction)
    {
        Id = id;
        BaseCurrency = baseCurrency;
        QuoteCurrency = quoteCurrency;
        EntryAtUtc = entryAtUtc;
        EntryPrice = entryPrice;
        Direction = direction;
    }

    public static Entry Create(
        Currency baseCurrency,
        Currency quoteCurrency,
        DateTime entryAtUtc,
        decimal entryPrice,
        Direction direction,
        IReadOnlyList<EntryFundamentalFeatureSnapshot> fundamentalSnapshot,
        IReadOnlyList<EntryTechnicalFeatureSnapshot> technicalSnapshot,
        decimal? formula1W,
        decimal? formula1M,
        decimal? formula3M,
        decimal? formula1Y)
    {
        _ = new CurrencyPair(baseCurrency, quoteCurrency); // throws if not one of the 28 supported pairs

        var normalizedAt = DateTimeUtc.Normalize(entryAtUtc);
        Validate(normalizedAt, entryPrice, direction);

        var entry = new Entry(Guid.NewGuid(), baseCurrency, quoteCurrency, normalizedAt, entryPrice, direction)
        {
            Formula1W = formula1W,
            Formula1M = formula1M,
            Formula3M = formula3M,
            Formula1Y = formula1Y
        };

        entry._fundamentalSnapshot.AddRange(fundamentalSnapshot);
        entry._technicalSnapshot.AddRange(technicalSnapshot);

        return entry;
    }

    private static void Validate(DateTime normalizedEntryAtUtc, decimal entryPrice, Direction direction)
    {
        if (!Enum.IsDefined(direction))
        {
            throw new FxEdgeValidationException($"'{direction}' is not a recognized direction.");
        }

        if (entryPrice <= 0)
        {
            throw new FxEdgeValidationException("EntryPrice must be greater than zero.");
        }

        if (normalizedEntryAtUtc > DateTime.UtcNow)
        {
            throw new FxEdgeValidationException("EntryAtUtc cannot be in the future.");
        }
    }
}
