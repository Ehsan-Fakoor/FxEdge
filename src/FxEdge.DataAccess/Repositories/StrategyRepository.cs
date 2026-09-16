using FxEdge.Business.Abstractions;
using FxEdge.Business.Entities;
using Microsoft.EntityFrameworkCore;

namespace FxEdge.DataAccess.Repositories;

/// <inheritdoc cref="IStrategyRepository"/>
public sealed class StrategyRepository : IStrategyRepository
{
    private readonly FxEdgeDbContext _dbContext;

    public StrategyRepository(FxEdgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Strategy strategy, CancellationToken ct = default) =>
        await _dbContext.Strategies.AddAsync(strategy, ct);

    public Task<Strategy?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _dbContext.Strategies.SingleOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<Strategy>> QueryAsync(CancellationToken ct = default) =>
        await _dbContext.Strategies
            .OrderBy(s => s.Name)
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _dbContext.SaveChangesAsync(ct);
}
