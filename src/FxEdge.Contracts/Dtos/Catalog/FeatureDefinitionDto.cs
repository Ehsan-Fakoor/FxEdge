using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Catalog;

/// <summary>
/// The fixed, human-readable definition of one fundamental feature.
/// </summary>
/// <param name="Polarity">
/// +1 if a higher reading is generally supportive of the currency (bullish),
/// -1 if a higher reading is generally unsupportive of the currency (bearish).
/// Used to sign the Base-minus-Quote differential consistently across all 14 features.
/// </param>
public sealed record FeatureDefinitionDto(
    FundamentalFeature Feature,
    string DisplayNameFa,
    string DisplayNameEn,
    int Polarity);
