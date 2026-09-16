namespace FxEdge.Contracts.Dtos.Strategies;

/// <summary>
/// A named, ATR-based scaling strategy definition: how far apart successive entries
/// are placed, each entry's individual take-profit, the basket's overall stop-loss,
/// and how many entries are allowed in total.
/// </summary>
public sealed record StrategyDto(
    Guid Id,
    string Name,
    decimal EntrySpacingAtr,
    decimal TakeProfitPerEntryAtr,
    decimal OverallStopLossAtr,
    int MaxEntries);
