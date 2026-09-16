using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Observations;

/// <summary>
/// Request to correct a mistakenly entered observation. This overwrites every field of
/// the existing record in place (no audit trail is kept for the correction itself).
/// This is distinct from normal historical accumulation: a new AnnouncementAtUtc value
/// entered through CreateObservationRequest always produces a new, independent record.
/// </summary>
public sealed record EditObservationRequest(
    Guid Id,
    Currency Currency,
    FundamentalFeature Feature,
    DateTime AnnouncementAtUtc,
    decimal? ForecastValue,
    decimal Value);
