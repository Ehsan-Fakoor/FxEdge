using FxEdge.Business.Catalog;
using FxEdge.Contracts.Dtos.Dataset;

namespace FxEdge.Business.Services;

/// <summary>
/// Computes DifferentialValue = Polarity * (BaseNormalizedValue - QuoteNormalizedValue)
/// for one feature. The normalized values (see FeatureNormalizer) are supplied by the
/// caller rather than resolved here, keeping this class stateless/static like before.
/// Null-propagates if either side's normalization is unavailable.
/// </summary>
internal static class DifferentialCalculator
{
    public static PairFeatureDifferentialDto Calculate(
        FeatureSnapshotDto baseFeature,
        FeatureSnapshotDto quoteFeature,
        decimal? baseNormalizedValue,
        decimal? quoteNormalizedValue)
    {
        var polarity = FundamentalFeatureCatalog.GetPolarity(baseFeature.Feature);

        decimal? differential = baseNormalizedValue.HasValue && quoteNormalizedValue.HasValue
            ? polarity * (baseNormalizedValue.Value - quoteNormalizedValue.Value)
            : null;

        return new PairFeatureDifferentialDto(
            baseFeature.Feature,
            polarity,
            baseFeature.Value,
            quoteFeature.Value,
            baseNormalizedValue,
            quoteNormalizedValue,
            differential);
    }
}
