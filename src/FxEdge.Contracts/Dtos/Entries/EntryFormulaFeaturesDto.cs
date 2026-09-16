namespace FxEdge.Contracts.Dtos.Entries;

/// <summary>
/// The four fixed Fundamental Formula Features computed for an Entry: 1 Week, 1 Month,
/// 3 Months, 1 Year. The exact calculation is not yet defined - every field is null
/// until the formulas are supplied, but the shape is already final so nothing else
/// needs to change once they are.
/// </summary>
public sealed record EntryFormulaFeaturesDto(
    decimal? Formula1W,
    decimal? Formula1M,
    decimal? Formula3M,
    decimal? Formula1Y);
