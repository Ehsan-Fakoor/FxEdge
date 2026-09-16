namespace FxEdge.Contracts.Enums;

/// <summary>
/// The fixed set of 16 fundamental features tracked per currency. The list is closed by
/// design (no dynamic/user-defined feature types). Display names and economic polarity
/// (whether a higher reading is generally supportive or unsupportive of the currency)
/// live in the single source of truth: FxEdge.Business.Catalog.FundamentalFeatureCatalog.
/// </summary>
public enum FundamentalFeature
{
    /// <summary>1. حجم مخارج واقعی به ازای هر نفر (Real Spending Per Capita)</summary>
    RealSpendingPerCapita = 1,

    /// <summary>2. نرخ رشد GDP (GDP Growth Rate)</summary>
    GdpGrowthRate = 2,

    /// <summary>3. نرخ بیکاری (Unemployment Rate)</summary>
    UnemploymentRate = 3,

    /// <summary>4. Inflation Rate</summary>
    InflationRate = 4,

    /// <summary>5. Core CPI</summary>
    CoreCpi = 5,

    /// <summary>6. نرخ بهره بانک مرکزی (Central Bank Interest Rate)</summary>
    CentralBankInterestRate = 6,

    /// <summary>7. نرخ بهره واقعی (Real Interest Rate). Derived, never entered directly: CentralBankInterestRate minus CoreCpi - see FxEdge.Business.Services.RealInterestRateDerivationService.</summary>
    RealInterestRate = 7,

    /// <summary>8. بازده اوراق 2 ساله (2-Year Bond Yield)</summary>
    TwoYearBondYield = 8,

    /// <summary>9. بازده اوراق 10 ساله (10-Year Bond Yield)</summary>
    TenYearBondYield = 9,

    /// <summary>10. Gov. Budget</summary>
    GovernmentBudget = 10,

    /// <summary>11. Debt/GDP</summary>
    DebtToGdp = 11,

    /// <summary>12. تراز تجاری (Trade Balance)</summary>
    TradeBalance = 12,

    /// <summary>13. انتظار بازار از نرخ بهره (Market Rate Expectation)</summary>
    MarketRateExpectation = 13,

    /// <summary>14. PMI</summary>
    Pmi = 14,

    /// <summary>15. تراز حساب جاری (Current Account)</summary>
    CurrentAccount = 15,

    /// <summary>16. رشد دستمزد (Wage Growth)</summary>
    WageGrowth = 16
}
