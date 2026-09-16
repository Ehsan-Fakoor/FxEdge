using FxEdge.Contracts.Enums;

namespace FxEdge.Business.Abstractions;

/// <summary>
/// The four fixed lookback periods the Fundamental Formula Features are computed over.
/// </summary>
public enum FormulaPeriod
{
    OneWeek,
    OneMonth,
    ThreeMonths,
    OneYear
}

/// <summary>
/// Computes one Fundamental Formula Feature as a fixed, weighted average of
/// Z(Feature) terms, where Z(Feature) is exactly the Polarity-adjusted Difference
/// already resolved for that feature in the Entry's Fundamental Snapshot
/// (EntryFundamentalFeatureSnapshot.Difference / PairFeatureDifferentialDto.DifferentialValue).
/// Pure computation, no I/O - EntryService always calls this immediately after building
/// the Entry's FundamentalSnapshot, passing that snapshot's Differences directly.
/// Returns null if any feature the requested period needs has a null Difference (the
/// same "don't fabricate a partial result" rule as everywhere else in this system).
/// </summary>
public interface IFormulaFeatureCalculator
{
    decimal? Calculate(IReadOnlyDictionary<FundamentalFeature, decimal?> differencesByFeature, FormulaPeriod period);
}
