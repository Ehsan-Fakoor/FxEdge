using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.EntryResults;

/// <summary>
/// One Entry's manually-observed outcome for one Horizon: how far price moved in its
/// favor (MFE) and against it (MAE), where it ended up (ReturnAtHorizonEndAtr), and
/// which of MFE/MAE happened first - all expressed in ATR units (using the ATR
/// recorded on the Entry's Technical Snapshot). Read off the chart by the user after
/// the horizon has elapsed - not computed by the app (there is no price history store
/// in this system to compute it from).
/// </summary>
/// <param name="ReturnAtHorizonEndAtr">
/// (ClosingPrice - EntryPrice) / ATR at the end of the horizon, signed the same way as
/// MFE/MAE (positive = in profit, negative = at a loss). Lets StrategySimulation
/// resolve an outcome for Entries that reached neither the take-profit nor the
/// stop-loss threshold during the horizon.
/// </param>
/// <param name="FirstExtremeReached">
/// Whether MFE or MAE occurred first within the horizon - resolves StrategySimulation's
/// tie-breaking ambiguity when both a strategy's take-profit and stop-loss levels fall
/// within [MAE, MFE].
/// </param>
public sealed record EntryResultDto(
    Guid EntryId,
    ResultHorizon Horizon,
    decimal MaximumFavorableExcursionAtr,
    decimal MaximumAdverseExcursionAtr,
    decimal ReturnAtHorizonEndAtr,
    PriceExtreme FirstExtremeReached);
