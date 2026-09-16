using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Observations;

/// <summary>
/// A single, immutable-in-history fundamental data point: one currency, one feature,
/// one announcement. This is what gets returned to callers after create/edit/get/query.
/// </summary>
/// <param name="Id">Application-generated identifier (Guid.NewGuid(), never DB-generated).</param>
/// <param name="Currency">The currency this observation belongs to.</param>
/// <param name="Feature">The fundamental feature being observed.</param>
/// <param name="AnnouncementAtUtc">The UTC date+time the figure was announced/published.</param>
/// <param name="ForecastValue">The market forecast that existed for this announcement, if one was ever published for this feature.</param>
/// <param name="Value">The actual published value.</param>
public sealed record ObservationDto(
    Guid Id,
    Currency Currency,
    FundamentalFeature Feature,
    DateTime AnnouncementAtUtc,
    decimal? ForecastValue,
    decimal Value);
