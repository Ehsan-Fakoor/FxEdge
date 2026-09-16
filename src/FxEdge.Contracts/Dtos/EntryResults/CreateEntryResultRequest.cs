using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.EntryResults;

/// <summary>
/// Request to record an Entry's MFE/MAE/closing return/extreme order for one Horizon.
/// EntryId is supplied via the route, not this body.
/// </summary>
public sealed record CreateEntryResultRequest(
    ResultHorizon Horizon,
    decimal MaximumFavorableExcursionAtr,
    decimal MaximumAdverseExcursionAtr,
    decimal ReturnAtHorizonEndAtr,
    PriceExtreme FirstExtremeReached);
