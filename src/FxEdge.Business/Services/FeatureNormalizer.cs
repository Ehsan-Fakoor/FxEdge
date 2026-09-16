using FxEdge.Business.Abstractions;
using FxEdge.Contracts.Enums;

namespace FxEdge.Business.Services;

/// <summary>
/// Computes the point-in-time Z-score of a Currency+Feature's "current" value against
/// its own historical distribution: (Value - Mean) / SampleStdDev, where Mean and
/// SampleStdDev are computed exclusively from that same Currency+Feature's
/// observations announced at or before AsOfUtc (the current value itself is one of
/// them). This is what feeds FxEdge.Business.Services.DifferentialCalculator - raw
/// Value/ForecastValue/PreviousValue and their derived ActualVs* figures (produced by
/// PointInTimeResolver) are left untouched everywhere else.
///
/// Kept point-in-time for the same reason as everything else in this system: computing
/// Mean/StdDev from observations announced after AsOfUtc would leak future information
/// into a supposedly historical figure.
/// </summary>
internal sealed class FeatureNormalizer
{
    private readonly IObservationRepository _repository;

    public FeatureNormalizer(IObservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<decimal?> ResolveNormalizedValueAsync(Currency currency, FundamentalFeature feature, DateTime asOfUtc, CancellationToken ct)
    {
        // fromUtc: null intentionally - there is no lookback window, the whole
        // available history up to AsOfUtc contributes to Mean/StdDev.
        var observations = await _repository.QueryAsync(currency, feature, fromUtc: null, toUtc: asOfUtc, ct);
        if (observations.Count < 2)
        {
            // A sample standard deviation is mathematically undefined for fewer than
            // two data points - null rather than a fabricated/divide-by-zero result.
            return null;
        }

        var values = observations.Select(o => o.Value).ToList();
        var mean = values.Average();

        var sumOfSquaredDeviations = values.Sum(v => (v - mean) * (v - mean));
        var sampleVariance = sumOfSquaredDeviations / (values.Count - 1);
        var stdDev = (decimal)Math.Sqrt((double)sampleVariance);

        if (stdDev == 0m)
        {
            // Every observation so far has been identical - no variation to normalize against.
            return null;
        }

        var current = observations.OrderByDescending(o => o.AnnouncementAtUtc).First();
        return (current.Value - mean) / stdDev;
    }
}
