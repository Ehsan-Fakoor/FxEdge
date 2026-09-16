namespace FxEdge.Contracts.Enums;

/// <summary>
/// How a single Entry resolved within a StrategySimulation run, for one horizon.
/// </summary>
public enum SimulationOutcome
{
    /// <summary>MFE reached the strategy's take-profit threshold (and MAE did not reach the stop-loss threshold first).</summary>
    TookProfit,

    /// <summary>MAE reached the strategy's stop-loss threshold (and MFE did not reach the take-profit threshold first).</summary>
    HitStopLoss,

    /// <summary>Neither threshold was reached by the end of the horizon.</summary>
    ClosedAtHorizonEnd
}
