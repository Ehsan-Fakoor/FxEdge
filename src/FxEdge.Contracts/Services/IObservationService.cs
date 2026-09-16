using FxEdge.Contracts.Dtos.Observations;

namespace FxEdge.Contracts.Services;

/// <summary>
/// Data-entry surface for fundamental observations: create (append), edit (overwrite an
/// existing record to fix a mistake), and read/query. There is intentionally no delete
/// in this phase - it was not part of the requested scope.
/// </summary>
public interface IObservationService
{
    Task<ObservationDto> CreateAsync(CreateObservationRequest request, CancellationToken ct = default);

    Task<ObservationDto> EditAsync(EditObservationRequest request, CancellationToken ct = default);

    Task<ObservationDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<ObservationDto>> QueryAsync(ObservationQuery query, CancellationToken ct = default);
}
