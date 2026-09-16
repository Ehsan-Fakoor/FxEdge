using FxEdge.Business.Abstractions;
using FxEdge.Business.Entities;
using FxEdge.Contracts.Enums;
using Microsoft.EntityFrameworkCore;

namespace FxEdge.DataAccess.Repositories;

/// <inheritdoc cref="IEntryResultRepository"/>
public sealed class EntryResultRepository : IEntryResultRepository
{
    private readonly FxEdgeDbContext _dbContext;

    public EntryResultRepository(FxEdgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(EntryResult result, CancellationToken ct = default) =>
        await _dbContext.EntryResults.AddAsync(result, ct);

    public Task<EntryResult?> GetAsync(Guid entryId, ResultHorizon horizon, CancellationToken ct = default) =>
        _dbContext.EntryResults.SingleOrDefaultAsync(r => r.EntryId == entryId && r.Horizon == horizon, ct);

    public async Task<IReadOnlyList<EntryResult>> QueryByEntryAsync(Guid entryId, CancellationToken ct = default) =>
        await _dbContext.EntryResults
            .Where(r => r.EntryId == entryId)
            .OrderBy(r => r.Horizon)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<EntryResult>> QueryByHorizonAsync(ResultHorizon horizon, CancellationToken ct = default) =>
        await _dbContext.EntryResults
            .Where(r => r.Horizon == horizon)
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _dbContext.SaveChangesAsync(ct);
}
