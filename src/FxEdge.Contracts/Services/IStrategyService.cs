using FxEdge.Contracts.Dtos.Strategies;

namespace FxEdge.Contracts.Services;

/// <summary>
/// CRUD surface for Strategy parameter sets (Layer 2). Unlike Entry/Observation
/// history, a Strategy is reusable configuration, so it supports Edit directly rather
/// than only correction-in-place.
/// </summary>
public interface IStrategyService
{
    Task<StrategyDto> CreateAsync(CreateStrategyRequest request, CancellationToken ct = default);

    Task<StrategyDto> EditAsync(Guid id, EditStrategyRequest request, CancellationToken ct = default);

    Task<StrategyDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<StrategyDto>> QueryAsync(CancellationToken ct = default);
}
