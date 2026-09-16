using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Catalog;

/// <summary>
/// The fixed, human-readable definition of one technical feature.
/// </summary>
public sealed record TechnicalFeatureDefinitionDto(
    TechnicalFeature Feature,
    string DisplayNameFa,
    string DisplayNameEn);
