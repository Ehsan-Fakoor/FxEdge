using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Observations;

/// <summary>
/// Request to record a newly published fundamental figure. Value is always required:
/// an observation is only entered once the actual figure has been published.
/// </summary>
public sealed record CreateObservationRequest(
    Currency Currency,
    FundamentalFeature Feature,
    DateTime AnnouncementAtUtc,
    decimal Value);
