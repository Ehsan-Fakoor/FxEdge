using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Entries;

/// <summary>
/// A manually marked chart entry with everything captured about it at that moment:
/// the frozen 16-feature fundamental snapshot (Base+Quote+Differential), the 7
/// user-entered technical readings, and the 4 formula features. Returned by
/// create and get-by-id; QueryAsync returns the lighter EntrySummaryDto instead.
/// </summary>
/// <param name="AtrValue">
/// The ATR reading frozen on this Entry's TechnicalSnapshot at entry time - the
/// reference every "...Atr"-suffixed quantity elsewhere (EntryResult, EntryDailyCandle,
/// and Strategy parameters once simulated) is a multiple of. Null if ATR was never
/// entered for this Entry.
/// </param>
public sealed record EntryDto(
    Guid Id,
    Currency BaseCurrency,
    Currency QuoteCurrency,
    string PairSymbol,
    DateTime EntryAtUtc,
    decimal EntryPrice,
    Direction Direction,
    IReadOnlyList<EntryFundamentalFeatureSnapshotDto> FundamentalSnapshot,
    IReadOnlyList<TechnicalFeatureValueDto> Technical,
    EntryFormulaFeaturesDto Formulas,
    decimal? AtrValue);
