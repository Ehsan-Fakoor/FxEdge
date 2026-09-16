using FxEdge.Business.Entities;

namespace FxEdge.Business.Abstractions;

public interface IStrategyRepository
{
    Task AddAsync(Strategy strategy, CancellationToken ct = default);

    Task<Strategy?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Strategy>> QueryAsync(CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
