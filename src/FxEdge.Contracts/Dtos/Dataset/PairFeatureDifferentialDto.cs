using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Dataset;

/// <summary>
/// The computed fundamental differential of one feature for a currency pair:
/// DifferentialValue = Polarity * (BaseNormalizedValue - QuoteNormalizedValue). Both
/// raw values (BaseValue/QuoteValue) and their point-in-time Z-score normalizations
/// (BaseNormalizedValue/QuoteNormalizedValue - see
/// FxEdge.Business.Services.FeatureNormalizer) are exposed for auditability. Null
/// wherever the underlying value or its normalization is not yet known. Never
/// persisted - always derived from the two underlying currencies' observations.
/// </summary>
public sealed record PairFeatureDifferentialDto(
    FundamentalFeature Feature,
    int Polarity,
    decimal? BaseValue,
    decimal? QuoteValue,
    decimal? BaseNormalizedValue,
    decimal? QuoteNormalizedValue,
    decimal? DifferentialValue);
