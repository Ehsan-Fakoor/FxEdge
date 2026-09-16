namespace FxEdge.Contracts.Dtos.Strategies;

/// <summary>
/// Request to update an existing Strategy's parameters. Id is supplied via the route,
/// not this body. Unlike Entry/Observation history, a Strategy is a reusable
/// configuration rather than a historical record, so editing it in place (rather than
/// only correcting mistakes) is expected as parameters get tuned over time.
/// </summary>
public sealed record EditStrategyRequest(
    string Name,
    decimal EntrySpacingAtr,
    decimal TakeProfitPerEntryAtr,
    decimal OverallStopLossAtr,
    int MaxEntries);
