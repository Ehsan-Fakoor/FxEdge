using FxEdge.Business.Abstractions;
using FxEdge.Contracts.Enums;

namespace FxEdge.Business.Services;

/// <inheritdoc cref="IFormulaFeatureCalculator"/>
/// <remarks>
/// Each formula is a fixed weighted average (weights sum to 1.00) of
/// Z(Feature) = the Polarity-adjusted Difference already resolved for that feature.
/// The weight tables below are the exact, confirmed specification - do not reorder or
/// rebalance them without updating the spec they came from.
/// </remarks>
public sealed class FormulaFeatureCalculator : IFormulaFeatureCalculator
{
    private static readonly IReadOnlyList<(FundamentalFeature Feature, decimal Weight)> Weights1W = new (FundamentalFeature, decimal)[]
    {
        (FundamentalFeature.MarketRateExpectation, 0.40m),
        (FundamentalFeature.TwoYearBondYield, 0.25m),
        (FundamentalFeature.CentralBankInterestRate, 0.20m),
        (FundamentalFeature.Pmi, 0.15m),
    };

    private static readonly IReadOnlyList<(FundamentalFeature Feature, decimal Weight)> Weights1M = new (FundamentalFeature, decimal)[]
    {
        (FundamentalFeature.MarketRateExpectation, 0.25m),
        (FundamentalFeature.CentralBankInterestRate, 0.20m),
        (FundamentalFeature.CoreCpi, 0.18m),
        (FundamentalFeature.TwoYearBondYield, 0.15m),
        (FundamentalFeature.Pmi, 0.12m),
        (FundamentalFeature.UnemploymentRate, 0.10m),
    };

    private static readonly IReadOnlyList<(FundamentalFeature Feature, decimal Weight)> Weights3M = new (FundamentalFeature, decimal)[]
    {
        (FundamentalFeature.GdpGrowthRate, 0.17m),
        (FundamentalFeature.RealInterestRate, 0.15m),
        (FundamentalFeature.CentralBankInterestRate, 0.13m),
        (FundamentalFeature.TenYearBondYield, 0.13m),
        (FundamentalFeature.TradeBalance, 0.12m),
        (FundamentalFeature.WageGrowth, 0.10m),
        (FundamentalFeature.UnemploymentRate, 0.10m),
        (FundamentalFeature.Pmi, 0.10m),
    };

    private static readonly IReadOnlyList<(FundamentalFeature Feature, decimal Weight)> Weights1Y = new (FundamentalFeature, decimal)[]
    {
        (FundamentalFeature.GdpGrowthRate, 0.20m),
        (FundamentalFeature.RealInterestRate, 0.16m),
        (FundamentalFeature.CurrentAccount, 0.15m),
        (FundamentalFeature.TenYearBondYield, 0.12m),
        (FundamentalFeature.DebtToGdp, 0.10m),
        (FundamentalFeature.GovernmentBudget, 0.10m),
        (FundamentalFeature.InflationRate, 0.10m),
        (FundamentalFeature.RealSpendingPerCapita, 0.07m),
    };

    public decimal? Calculate(IReadOnlyDictionary<FundamentalFeature, decimal?> differencesByFeature, FormulaPeriod period)
    {
        var weights = period switch
        {
            FormulaPeriod.OneWeek => Weights1W,
            FormulaPeriod.OneMonth => Weights1M,
            FormulaPeriod.ThreeMonths => Weights3M,
            FormulaPeriod.OneYear => Weights1Y,
            _ => throw new ArgumentOutOfRangeException(nameof(period), period, "Unknown formula period.")
        };

        decimal total = 0m;
        foreach (var (feature, weight) in weights)
        {
            if (!differencesByFeature.TryGetValue(feature, out var difference) || difference is not { } value)
            {
                // One required feature's Difference is unavailable - the whole result
                // is null rather than a silently partial/fabricated number.
                return null;
            }

            total += weight * value;
        }

        return total;
    }
}
