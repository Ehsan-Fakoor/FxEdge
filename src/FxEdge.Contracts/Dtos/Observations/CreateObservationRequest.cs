using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Observations;

/// <summary>
/// Request to record a newly published fundamental figure. Value is always required:
/// an observation is only entered once the actual figure has been published, so there
/// is no forecast-only / two-phase entry in this system. ForecastValue is optional -
/// some features (or specific announcements) genuinely never have a published market
/// forecast; omit it (or send null) rather than fabricating a number.
/// </summary>
public sealed record CreateObservationRequest(
    Currency Currency,
    FundamentalFeature Feature,
    DateTime AnnouncementAtUtc,
    decimal? ForecastValue,
    decimal Value);
