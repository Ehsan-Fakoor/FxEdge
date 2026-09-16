using FxEdge.Contracts.Dtos.Dataset;
using FxEdge.Contracts.Enums;

namespace FxEdge.Business.Entities;

/// <summary>
/// One feature's fundamental state as it stood at the moment its parent Entry was
/// created, frozen permanently. Every value here is copied once from a point-in-time
/// resolution against data announced at or before the Entry's EntryAtUtc, and never
/// recomputed afterward - a later correction to historical Observation data must not
/// retroactively change an Entry that already happened.
///
/// A pure data holder: all construction/validation happens in EntryService, which is
/// the only place besides EF Core allowed to create one (hence the internal constructor).
/// </summary>
public sealed class EntryFundamentalFeatureSnapshot
{
    public Guid EntryId { get; private set; }
    public FundamentalFeature Feature { get; private set; }
    public int Polarity { get; private set; }

    public decimal? BaseValue { get; private set; }
    public decimal? BasePreviousValue { get; private set; }
    public decimal? BaseActualVsPrevious { get; private set; }
    public DateTime? BaseAnnouncementAtUtc { get; private set; }

    /// <summary>(BaseValue - Mean) / SampleStdDev, point-in-time, frozen at snapshot time. Feeds Difference - see FxEdge.Business.Services.FeatureNormalizer.</summary>
    public decimal? BaseNormalizedValue { get; private set; }

    public decimal? QuoteValue { get; private set; }
    public decimal? QuotePreviousValue { get; private set; }
    public decimal? QuoteActualVsPrevious { get; private set; }
    public DateTime? QuoteAnnouncementAtUtc { get; private set; }

    /// <summary>The same Z-score normalization as BaseNormalizedValue, for QuoteValue.</summary>
    public decimal? QuoteNormalizedValue { get; private set; }

    /// <summary>Polarity * (BaseNormalizedValue - QuoteNormalizedValue), frozen at snapshot time.</summary>
    public decimal? Difference { get; private set; }

    // EF Core materialization constructor.
    private EntryFundamentalFeatureSnapshot()
    {
    }

    internal EntryFundamentalFeatureSnapshot(
        FundamentalFeature feature,
        int polarity,
        FeatureSnapshotDto baseFeature,
        FeatureSnapshotDto quoteFeature,
        decimal? baseNormalizedValue,
        decimal? quoteNormalizedValue,
        decimal? difference)
    {
        Feature = feature;
        Polarity = polarity;

        BaseValue = baseFeature.Value;
        BasePreviousValue = baseFeature.PreviousValue;
        BaseActualVsPrevious = baseFeature.ActualVsPrevious;
        BaseAnnouncementAtUtc = baseFeature.AnnouncementAtUtc;
        BaseNormalizedValue = baseNormalizedValue;

        QuoteValue = quoteFeature.Value;
        QuotePreviousValue = quoteFeature.PreviousValue;
        QuoteActualVsPrevious = quoteFeature.ActualVsPrevious;
        QuoteAnnouncementAtUtc = quoteFeature.AnnouncementAtUtc;
        QuoteNormalizedValue = quoteNormalizedValue;

        Difference = difference;
    }
}
