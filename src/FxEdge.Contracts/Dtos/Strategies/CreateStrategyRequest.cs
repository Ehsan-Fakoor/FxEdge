namespace FxEdge.Contracts.Dtos.Strategies;

public sealed record CreateStrategyRequest(
    string Name,
    decimal EntrySpacingAtr,
    decimal TakeProfitPerEntryAtr,
    decimal OverallStopLossAtr,
    int MaxEntries);
