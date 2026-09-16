using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Dataset;

/// <summary>
/// The resolved, point-in-time state of a single fundamental feature for a single
/// currency as of a given date. All numeric fields are nullable: if no observation for
/// this Currency+Feature had been announced yet as of AsOf, every field is null. This
/// keeps the shape deterministic and leakage-free (only ever built from data whose
/// AnnouncementAtUtc is at or before AsOf) while still allowing a fixed-width feature
/// vector per currency for downstream dataset construction.
/// </summary>
/// <param name="Value">The actual value of the observation currently in effect (carried forward until the next announcement).</param>
/// <param name="PreviousValue">The actual value of the observation immediately preceding the current one, if any.</param>
/// <param name="ActualVsPrevious">Value - PreviousValue (the change from the prior reading), null if there is no prior reading.</param>
/// <param name="AnnouncementAtUtc">When the currently-effective observation was announced.</param>
public sealed record FeatureSnapshotDto(
    FundamentalFeature Feature,
    decimal? Value,
    decimal? PreviousValue,
    decimal? ActualVsPrevious,
    DateTime? AnnouncementAtUtc);
