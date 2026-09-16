using FxEdge.Business.Abstractions;
using FxEdge.Business.Entities;
using FxEdge.Contracts.Dtos.Observations;
using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;
using FxEdge.Contracts.Services;

namespace FxEdge.Business.Services;

/// <inheritdoc cref="IObservationService"/>
public sealed class ObservationService : IObservationService
{
    private readonly IObservationRepository _repository;
    private readonly RealInterestRateDerivationService _realInterestRateDerivation;

    public ObservationService(IObservationRepository repository)
    {
        _repository = repository;
        _realInterestRateDerivation = new RealInterestRateDerivationService(repository);
    }

    public async Task<ObservationDto> CreateAsync(CreateObservationRequest request, CancellationToken ct = default)
    {
        EnsureNotDerivedFeature(request.Feature);

        var duplicate = await _repository.ExistsAsync(request.Currency, request.Feature, request.AnnouncementAtUtc, excludeId: null, ct);
        if (duplicate)
        {
            throw new FxEdgeConflictException(
                $"An observation for {request.Currency}/{request.Feature} at {request.AnnouncementAtUtc:O} already exists. Use edit to correct it instead.");
        }

        var observation = FundamentalObservation.Create(
            request.Currency, request.Feature, request.AnnouncementAtUtc, request.ForecastValue, request.Value);

        await _repository.AddAsync(observation, ct);
        await _repository.SaveChangesAsync(ct);

        await DeriveIfTriggerAsync(request.Currency, request.Feature, request.AnnouncementAtUtc, ct);

        return ToDto(observation);
    }

    public async Task<ObservationDto> EditAsync(EditObservationRequest request, CancellationToken ct = default)
    {
        EnsureNotDerivedFeature(request.Feature);

        var observation = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new FxEdgeNotFoundException($"Observation '{request.Id}' was not found.");

        var duplicate = await _repository.ExistsAsync(request.Currency, request.Feature, request.AnnouncementAtUtc, excludeId: request.Id, ct);
        if (duplicate)
        {
            throw new FxEdgeConflictException(
                $"Another observation for {request.Currency}/{request.Feature} at {request.AnnouncementAtUtc:O} already exists.");
        }

        observation.Overwrite(request.Currency, request.Feature, request.AnnouncementAtUtc, request.ForecastValue, request.Value);
        await _repository.SaveChangesAsync(ct);

        await DeriveIfTriggerAsync(request.Currency, request.Feature, request.AnnouncementAtUtc, ct);

        return ToDto(observation);
    }

    public async Task<ObservationDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var observation = await _repository.GetByIdAsync(id, ct);
        return observation is null ? null : ToDto(observation);
    }

    public async Task<IReadOnlyList<ObservationDto>> QueryAsync(ObservationQuery query, CancellationToken ct = default)
    {
        var results = await _repository.QueryAsync(query.Currency, query.Feature, query.FromUtc, query.ToUtc, ct);
        return results.Select(ToDto).ToList();
    }

    /// <summary>RealInterestRate is derived, never entered directly - see RealInterestRateDerivationService.</summary>
    private static void EnsureNotDerivedFeature(FundamentalFeature feature)
    {
        if (feature == FundamentalFeature.RealInterestRate)
        {
            throw new FxEdgeValidationException(
                "RealInterestRate cannot be entered or edited directly - it is automatically derived as " +
                "CentralBankInterestRate minus CoreCpi whenever either of those is recorded. Enter those two " +
                "features instead.");
        }
    }

    private async Task DeriveIfTriggerAsync(Currency currency, FundamentalFeature feature, DateTime announcementAtUtc, CancellationToken ct)
    {
        if (!_realInterestRateDerivation.IsTrigger(feature))
        {
            return;
        }

        await _realInterestRateDerivation.DeriveAsync(currency, announcementAtUtc, ct);
        await _repository.SaveChangesAsync(ct);
    }

    private static ObservationDto ToDto(FundamentalObservation observation) => new(
        observation.Id,
        observation.Currency,
        observation.Feature,
        observation.AnnouncementAtUtc,
        observation.ForecastValue,
        observation.Value);
}
