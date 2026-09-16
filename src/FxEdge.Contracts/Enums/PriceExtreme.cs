namespace FxEdge.Contracts.Enums;

/// <summary>
/// Which of an Entry's two price excursions (Maximum Favorable / Maximum Adverse) was
/// reached first within a Horizon. Resolves the tie-breaking ambiguity for
/// StrategySimulation: if a strategy's take-profit and stop-loss are both within
/// [MAE, MFE], whichever extreme was reached first determines which threshold the
/// strategy would have hit first.
/// </summary>
public enum PriceExtreme
{
    /// <summary>The Maximum Favorable Excursion was reached before the Maximum Adverse Excursion.</summary>
    Favorable,

    /// <summary>The Maximum Adverse Excursion was reached before the Maximum Favorable Excursion.</summary>
    Adverse
}
