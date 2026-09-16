using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.EntryResults;

/// <summary>
/// Request to correct an already-recorded EntryResult in place (e.g. a misread chart
/// value). EntryId and Horizon are supplied via the route, not this body.
/// </summary>
public sealed record EditEntryResultRequest(
    decimal MaximumFavorableExcursionAtr,
    decimal MaximumAdverseExcursionAtr,
    decimal ReturnAtHorizonEndAtr,
    PriceExtreme FirstExtremeReached);
