using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;

namespace FxEdge.Business.Catalog;

/// <summary>
/// The single, fixed source of truth for the 16 fundamental features: their real
/// display names and their economic Polarity. Nothing about the feature set is dynamic
/// or database-driven - this is deliberately a static, in-code configuration file.
/// </summary>
public static class FundamentalFeatureCatalog
{
    /// <param name="Polarity">
    /// +1 if a higher reading is generally supportive of the currency (bullish),
    /// -1 if a higher reading is generally unsupportive of the currency (bearish, e.g.
    /// Unemployment Rate or Debt/GDP). Used to sign the pair-level differential so that
    /// "positive differential" consistently means "favors the Base currency" for every
    /// feature.
    /// </param>
    public sealed record Definition(FundamentalFeature Feature, string DisplayNameFa, string DisplayNameEn, int Polarity);

    // Order intentionally matches the 16-item list as specified for the project.
    private static readonly IReadOnlyList<Definition> Definitions = new List<Definition>
    {
        new(FundamentalFeature.RealSpendingPerCapita, "حجم مخارج واقعی به ازای هر نفر", "Real Spending Per Capita", +1),
        new(FundamentalFeature.GdpGrowthRate, "نرخ رشد GDP", "GDP Growth Rate", +1),
        new(FundamentalFeature.UnemploymentRate, "نرخ بیکاری", "Unemployment Rate", -1),
        new(FundamentalFeature.InflationRate, "نرخ تورم", "Inflation Rate", +1),
        new(FundamentalFeature.CoreCpi, "Core CPI", "Core CPI", +1),
        new(FundamentalFeature.CentralBankInterestRate, "نرخ بهره بانک مرکزی", "Central Bank Interest Rate", +1),
        // RealInterestRate's *display* entry stays here like every other feature, but its
        // actual data is never entered directly - see RealInterestRateDerivationService.
        new(FundamentalFeature.RealInterestRate, "نرخ بهره واقعی", "Real Interest Rate", +1),
        new(FundamentalFeature.TwoYearBondYield, "بازده اوراق ۲ ساله", "2-Year Bond Yield", +1),
        new(FundamentalFeature.TenYearBondYield, "بازده اوراق ۱۰ ساله", "10-Year Bond Yield", +1),
        new(FundamentalFeature.GovernmentBudget, "Gov. Budget", "Government Budget (Balance)", +1),
        new(FundamentalFeature.DebtToGdp, "Debt/GDP", "Debt to GDP", -1),
        new(FundamentalFeature.TradeBalance, "تراز تجاری", "Trade Balance", +1),
        new(FundamentalFeature.MarketRateExpectation, "انتظار بازار از نرخ بهره", "Market Rate Expectation", +1),
        new(FundamentalFeature.Pmi, "PMI", "PMI", +1),
        new(FundamentalFeature.CurrentAccount, "تراز حساب جاری", "Current Account", +1),
        new(FundamentalFeature.WageGrowth, "رشد دستمزد", "Wage Growth", +1),
    };

    private static readonly IReadOnlyDictionary<FundamentalFeature, Definition> ByFeature =
        Definitions.ToDictionary(d => d.Feature);

    /// <summary>All 16 definitions, in fixed order. Used to build fixed-width feature vectors.</summary>
    public static IReadOnlyList<Definition> All => Definitions;

    public static Definition Get(FundamentalFeature feature)
    {
        if (!ByFeature.TryGetValue(feature, out var definition))
        {
            throw new FxEdgeValidationException($"No catalog definition exists for feature '{feature}'.");
        }

        return definition;
    }

    public static int GetPolarity(FundamentalFeature feature) => Get(feature).Polarity;
}
