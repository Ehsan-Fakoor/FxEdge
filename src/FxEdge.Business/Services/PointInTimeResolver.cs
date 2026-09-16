using FxEdge.Business.Abstractions;
using FxEdge.Contracts.Dtos.Dataset;
using FxEdge.Contracts.Enums;

namespace FxEdge.Business.Services;

/// <summary>
/// Resolves the point-in-time state of a single Currency+Feature as of a given UTC
/// date: the observation in effect (carried forward until the next announcement),
/// plus the immediately preceding observation, used to derive "Previous" and the
/// change figure. Internal - only consumed by DatasetService.
/// </summary>
internal sealed class PointInTimeResolver
{
    private readonly IObservationRepository _repository;

    public PointInTimeResolver(IObservationRepository repository)
    {
        _repository = repository;
    }

    public async Task<FeatureSnapshotDto> ResolveAsync(Currency currency, FundamentalFeature feature, DateTime asOfUtc, CancellationToken ct)
    {
        var current = await _repository.GetLatestAsOfAsync(currency, feature, asOfUtc, ct);
        if (current is null)
        {
            // Nothing had been announced yet as of this date - every field stays null
            // rather than fabricating a value, to keep the dataset leakage-free.
            return new FeatureSnapshotDto(feature, null, null, null, null);
        }

        var previous = await _repository.GetLatestBeforeAsync(currency, feature, current.AnnouncementAtUtc, ct);

        decimal? previousValue = previous?.Value;
        decimal? actualVsPrevious = previousValue.HasValue ? current.Value - previousValue.Value : null;

        return new FeatureSnapshotDto(
            feature,
            current.Value,
            previousValue,
            actualVsPrevious,
            current.AnnouncementAtUtc);
    }
}
