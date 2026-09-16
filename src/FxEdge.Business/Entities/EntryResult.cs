using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;

namespace FxEdge.Business.Entities;

/// <summary>
/// One Entry's manually-observed outcome for one Horizon: MFE, MAE, the closing
/// return at horizon end, and which of MFE/MAE was reached first - all needed for
/// StrategySimulation to resolve an unambiguous outcome. Unlike Entry's frozen
/// snapshots (created atomically with the Entry itself), an EntryResult is recorded
/// separately and later - only once the horizon has actually elapsed - so it supports
/// Overwrite() for correcting a misread chart value, similar to FundamentalObservation.
/// </summary>
public sealed class EntryResult
{
    public Guid EntryId { get; private set; }
    public ResultHorizon Horizon { get; private set; }
    public decimal MaximumFavorableExcursionAtr { get; private set; }
    public decimal MaximumAdverseExcursionAtr { get; private set; }

    /// <summary>
    /// (ClosingPrice - EntryPrice) / ATR at the end of the horizon, signed in the
    /// trade's own favorable direction (positive = in profit, negative = at a loss) -
    /// the same convention as MFE/MAE. This is what lets StrategySimulation resolve an
    /// outcome for an Entry that reached neither the take-profit nor the stop-loss
    /// threshold during the horizon.
    /// </summary>
    public decimal ReturnAtHorizonEndAtr { get; private set; }

    /// <summary>
    /// Whether MFE or MAE occurred first within the horizon. Resolves StrategySimulation's
    /// tie-breaking ambiguity: if a strategy's take-profit and stop-loss are both within
    /// [MAE, MFE], whichever extreme happened first determines which threshold the
    /// strategy would have crossed first.
    /// </summary>
    public PriceExtreme FirstExtremeReached { get; private set; }

    // EF Core materialization constructor.
    private EntryResult()
    {
    }

    private EntryResult(Guid entryId, ResultHorizon horizon, decimal mfeAtr, decimal maeAtr, decimal returnAtHorizonEndAtr, PriceExtreme firstExtremeReached)
    {
        EntryId = entryId;
        Horizon = horizon;
        MaximumFavorableExcursionAtr = mfeAtr;
        MaximumAdverseExcursionAtr = maeAtr;
        ReturnAtHorizonEndAtr = returnAtHorizonEndAtr;
        FirstExtremeReached = firstExtremeReached;
    }

    public static EntryResult Create(Guid entryId, ResultHorizon horizon, decimal mfeAtr, decimal maeAtr, decimal returnAtHorizonEndAtr, PriceExtreme firstExtremeReached)
    {
        Validate(horizon, mfeAtr, maeAtr, returnAtHorizonEndAtr, firstExtremeReached);
        return new EntryResult(entryId, horizon, mfeAtr, maeAtr, returnAtHorizonEndAtr, firstExtremeReached);
    }

    public void Overwrite(decimal mfeAtr, decimal maeAtr, decimal returnAtHorizonEndAtr, PriceExtreme firstExtremeReached)
    {
        Validate(Horizon, mfeAtr, maeAtr, returnAtHorizonEndAtr, firstExtremeReached);
        MaximumFavorableExcursionAtr = mfeAtr;
        MaximumAdverseExcursionAtr = maeAtr;
        ReturnAtHorizonEndAtr = returnAtHorizonEndAtr;
        FirstExtremeReached = firstExtremeReached;
    }

    private static void Validate(ResultHorizon horizon, decimal mfeAtr, decimal maeAtr, decimal returnAtHorizonEndAtr, PriceExtreme firstExtremeReached)
    {
        if (!Enum.IsDefined(horizon))
        {
            throw new FxEdgeValidationException($"'{horizon}' is not a recognized result horizon.");
        }

        if (!Enum.IsDefined(firstExtremeReached))
        {
            throw new FxEdgeValidationException($"'{firstExtremeReached}' is not a recognized price extreme.");
        }

        // Both are excursion magnitudes (distance from entry price in ATR units), so
        // neither can be negative - direction is already implied by "Favorable"/"Adverse".
        if (mfeAtr < 0)
        {
            throw new FxEdgeValidationException("MaximumFavorableExcursionAtr cannot be negative.");
        }

        if (maeAtr < 0)
        {
            throw new FxEdgeValidationException("MaximumAdverseExcursionAtr cannot be negative.");
        }

        // Sanity check: the price could never have closed the horizon beyond the
        // extremes it reached during that same horizon.
        if (returnAtHorizonEndAtr > mfeAtr)
        {
            throw new FxEdgeValidationException(
                $"ReturnAtHorizonEndAtr ({returnAtHorizonEndAtr}) cannot exceed MaximumFavorableExcursionAtr ({mfeAtr}).");
        }

        if (returnAtHorizonEndAtr < -maeAtr)
        {
            throw new FxEdgeValidationException(
                $"ReturnAtHorizonEndAtr ({returnAtHorizonEndAtr}) cannot be more adverse than -MaximumAdverseExcursionAtr ({-maeAtr}).");
        }
    }
}
