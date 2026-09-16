using FxEdge.Business.Entities;

namespace FxEdge.Business.Abstractions;

public interface IStrategySimulationRepository
{
    Task AddAsync(StrategySimulation simulation, CancellationToken ct = default);

    /// <summary>Loads a simulation together with its full per-entry outcome breakdown.</summary>
    Task<StrategySimulation?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<StrategySimulation>> QueryByStrategyAsync(Guid strategyId, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
