using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;

namespace FxEdge.Business.Catalog;

/// <summary>
/// The single, fixed source of truth for the 7 technical features: their real display
/// names. No polarity concept applies here (unlike FundamentalFeatureCatalog) - these
/// belong to the traded instrument's chart itself, not to a single currency being
/// compared against another.
/// </summary>
public static class TechnicalFeatureCatalog
{
    public sealed record Definition(TechnicalFeature Feature, string DisplayNameFa, string DisplayNameEn);

    private static readonly IReadOnlyList<Definition> Definitions = new List<Definition>
    {
        new(TechnicalFeature.Rsi, "RSI", "RSI"),
        new(TechnicalFeature.Adx, "ADX", "ADX"),
        new(TechnicalFeature.Atr, "ATR", "ATR"),
        new(TechnicalFeature.CandleBody, "بدنه کندل (Close - Open)", "Candle Body (C-O)"),
        new(TechnicalFeature.CandleRange, "دامنه کندل (High - Low)", "Candle Range (H-L)"),
        new(TechnicalFeature.UpperShadow, "سایه بالایی", "Upper Shadow"),
        new(TechnicalFeature.LowerShadow, "سایه پایینی", "Lower Shadow"),
    };

    private static readonly IReadOnlyDictionary<TechnicalFeature, Definition> ByFeature =
        Definitions.ToDictionary(d => d.Feature);

    /// <summary>All 7 definitions, in fixed order.</summary>
    public static IReadOnlyList<Definition> All => Definitions;

    public static Definition Get(TechnicalFeature feature)
    {
        if (!ByFeature.TryGetValue(feature, out var definition))
        {
            throw new FxEdgeValidationException($"No catalog definition exists for technical feature '{feature}'.");
        }

        return definition;
    }
}
