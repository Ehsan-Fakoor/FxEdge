using FxEdge.Business.Abstractions;
using FxEdge.Business.Entities;
using Microsoft.EntityFrameworkCore;

namespace FxEdge.DataAccess.Repositories;

/// <inheritdoc cref="IStrategySimulationRepository"/>
public sealed class StrategySimulationRepository : IStrategySimulationRepository
{
    private readonly FxEdgeDbContext _dbContext;

    public StrategySimulationRepository(FxEdgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(StrategySimulation simulation, CancellationToken ct = default) =>
        await _dbContext.StrategySimulations.AddAsync(simulation, ct);

    public Task<StrategySimulation?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _dbContext.StrategySimulations
            .Include(s => s.EntryOutcomes)
            .SingleOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<StrategySimulation>> QueryByStrategyAsync(Guid strategyId, CancellationToken ct = default) =>
        await _dbContext.StrategySimulations
            .Where(s => s.StrategyId == strategyId)
            .Include(s => s.EntryOutcomes)
            .OrderByDescending(s => s.RunAtUtc)
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _dbContext.SaveChangesAsync(ct);
}
