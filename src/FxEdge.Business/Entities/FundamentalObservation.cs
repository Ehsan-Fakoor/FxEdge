using FxEdge.Business.Common;
using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;

namespace FxEdge.Business.Entities;

/// <summary>
/// One published fundamental figure: a single Currency, a single FundamentalFeature, at
/// a single AnnouncementAtUtc. This is the only entity FxEdge persists directly.
///
/// Historical semantics: normal data entry never modifies an existing observation - a
/// newly published figure for a later AnnouncementAtUtc always creates a brand-new
/// record via Create(), so that the market state at any past point in time can be
/// reconstructed. Overwrite() exists solely for correcting a mistaken entry (e.g. a
/// typo) in place; it is not part of the normal historical-accumulation flow.
/// </summary>
public sealed class FundamentalObservation
{
    public Guid Id { get; private set; }
    public Currency Currency { get; private set; }
    public FundamentalFeature Feature { get; private set; }

    /// <summary>UTC date+time the figure was announced/published.</summary>
    public DateTime AnnouncementAtUtc { get; private set; }

    public decimal Value { get; private set; }

    // EF Core materialization constructor.
    private FundamentalObservation()
    {
    }

    private FundamentalObservation(
        Guid id,
        Currency currency,
        FundamentalFeature feature,
        DateTime announcementAtUtc,
        decimal value)
    {
        Id = id;
        Currency = currency;
        Feature = feature;
        AnnouncementAtUtc = announcementAtUtc;
        Value = value;
    }

    /// <summary>
    /// Records a newly published figure as a brand-new, independent observation.
    /// The Id is generated here (Application-generated Guid), never by the database.
    /// Value is always required (an observation only exists once the actual figure is known).
    /// </summary>
    public static FundamentalObservation Create(
        Currency currency,
        FundamentalFeature feature,
        DateTime announcementAtUtc,
        decimal value)
    {
        var normalizedAt = DateTimeUtc.Normalize(announcementAtUtc);
        Validate(currency, feature, normalizedAt);

        return new FundamentalObservation(Guid.NewGuid(), currency, feature, normalizedAt, value);
    }

    /// <summary>
    /// Overwrites every field of this observation in place, to correct a mistaken entry.
    /// No history of the previous (incorrect) values is retained.
    /// </summary>
    public void Overwrite(
        Currency currency,
        FundamentalFeature feature,
        DateTime announcementAtUtc,
        decimal value)
    {
        var normalizedAt = DateTimeUtc.Normalize(announcementAtUtc);
        Validate(currency, feature, normalizedAt);

        Currency = currency;
        Feature = feature;
        AnnouncementAtUtc = normalizedAt;
        Value = value;
    }

    private static void Validate(Currency currency, FundamentalFeature feature, DateTime normalizedAnnouncementAtUtc)
    {
        if (!Enum.IsDefined(currency))
        {
            throw new FxEdgeValidationException($"'{currency}' is not a recognized currency.");
        }

        if (!Enum.IsDefined(feature))
        {
            throw new FxEdgeValidationException($"'{feature}' is not a recognized fundamental feature.");
        }

        // Value is always the actual published figure, so an observation cannot be
        // dated in the future relative to now.
        if (normalizedAnnouncementAtUtc > DateTime.UtcNow)
        {
            throw new FxEdgeValidationException(
                "AnnouncementAtUtc cannot be in the future: an observation is only recorded once the actual value has been published.");
        }
    }
}
