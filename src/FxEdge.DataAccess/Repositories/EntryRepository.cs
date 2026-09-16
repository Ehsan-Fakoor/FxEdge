using FxEdge.Business.Abstractions;
using FxEdge.Business.Entities;
using FxEdge.Contracts.Enums;
using Microsoft.EntityFrameworkCore;

namespace FxEdge.DataAccess.Repositories;

/// <inheritdoc cref="IEntryRepository"/>
public sealed class EntryRepository : IEntryRepository
{
    private readonly FxEdgeDbContext _dbContext;

    public EntryRepository(FxEdgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Entry entry, CancellationToken ct = default) =>
        await _dbContext.Entries.AddAsync(entry, ct);

    public Task<Entry?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _dbContext.Entries
            .Include(e => e.FundamentalSnapshot)
            .Include(e => e.TechnicalSnapshot)
            .SingleOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Entry>> QueryAsync(
        Currency? baseCurrency,
        Currency? quoteCurrency,
        DateTime? fromUtc,
        DateTime? toUtc,
        Direction? direction,
        CancellationToken ct = default)
    {
        // List views only need EntrySummaryDto, so the two owned collections are
        // intentionally not Included here - keeps the query light.
        var query = _dbContext.Entries.AsQueryable();

        if (baseCurrency.HasValue)
        {
            query = query.Where(e => e.BaseCurrency == baseCurrency.Value);
        }

        if (quoteCurrency.HasValue)
        {
            query = query.Where(e => e.QuoteCurrency == quoteCurrency.Value);
        }

        if (fromUtc.HasValue)
        {
            query = query.Where(e => e.EntryAtUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(e => e.EntryAtUtc <= toUtc.Value);
        }

        if (direction.HasValue)
        {
            query = query.Where(e => e.Direction == direction.Value);
        }

        return await query
            .OrderByDescending(e => e.EntryAtUtc)
            .ToListAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _dbContext.SaveChangesAsync(ct);
}
