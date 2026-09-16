using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Entries;

/// <summary>
/// One technical feature's value at Entry time. Value is nullable - not every reading
/// will be available for every Entry.
/// </summary>
public sealed record TechnicalFeatureValueDto(TechnicalFeature Feature, decimal? Value);
