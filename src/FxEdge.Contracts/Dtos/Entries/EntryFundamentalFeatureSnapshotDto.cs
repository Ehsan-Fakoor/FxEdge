using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Entries;

/// <summary>
/// One feature's fundamental state as it stood at Entry time, frozen permanently at
/// creation. Unlike FxEdge.Contracts.Dtos.Dataset.FundamentalDatasetRowDto (which is
/// always recomputed live from the current observation history), this is captured once
/// - using only observations announced at or before EntryAtUtc - and persisted as-is,
/// so a later correction to historical Observation data never retroactively changes an
/// Entry that already happened.
/// </summary>
/// <param name="Polarity">The feature's polarity, frozen at the moment of the snapshot.</param>
/// <param name="BaseNormalizedValue">
/// BaseValue's point-in-time Z-score: (BaseValue - Mean) / SampleStdDev, computed over
/// every Base-currency observation of this feature announced at or before EntryAtUtc
/// (including BaseValue itself). Null if fewer than 2 such observations exist yet, or
/// if their StdDev is 0. This - not the raw BaseValue - is what feeds Difference.
/// </param>
/// <param name="QuoteNormalizedValue">The same Z-score normalization, for QuoteValue.</param>
/// <param name="Difference">Polarity * (BaseNormalizedValue - QuoteNormalizedValue), computed once and stored.</param>
public sealed record EntryFundamentalFeatureSnapshotDto(
    FundamentalFeature Feature,
    int Polarity,
    decimal? BaseValue,
    decimal? BasePreviousValue,
    decimal? BaseActualVsPrevious,
    DateTime? BaseAnnouncementAtUtc,
    decimal? BaseNormalizedValue,
    decimal? QuoteValue,
    decimal? QuotePreviousValue,
    decimal? QuoteActualVsPrevious,
    DateTime? QuoteAnnouncementAtUtc,
    decimal? QuoteNormalizedValue,
    decimal? Difference);
