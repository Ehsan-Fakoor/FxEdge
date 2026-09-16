using FxEdge.Business.Abstractions;
using FxEdge.Business.Entities;
using Microsoft.EntityFrameworkCore;

namespace FxEdge.DataAccess.Repositories;

/// <inheritdoc cref="IEntryDailyCandleRepository"/>
public sealed class EntryDailyCandleRepository : IEntryDailyCandleRepository
{
    private readonly FxEdgeDbContext _dbContext;

    public EntryDailyCandleRepository(FxEdgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(EntryDailyCandle candle, CancellationToken ct = default) =>
        await _dbContext.EntryDailyCandles.AddAsync(candle, ct);

    public async Task AddRangeAsync(IEnumerable<EntryDailyCandle> candles, CancellationToken ct = default) =>
        await _dbContext.EntryDailyCandles.AddRangeAsync(candles, ct);

    public Task<EntryDailyCandle?> GetAsync(Guid entryId, int dayIndex, CancellationToken ct = default) =>
        _dbContext.EntryDailyCandles.SingleOrDefaultAsync(c => c.EntryId == entryId && c.DayIndex == dayIndex, ct);

    public async Task<IReadOnlyList<EntryDailyCandle>> QueryByEntryAsync(Guid entryId, CancellationToken ct = default) =>
        await _dbContext.EntryDailyCandles
            .Where(c => c.EntryId == entryId)
            .OrderBy(c => c.DayIndex)
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _dbContext.SaveChangesAsync(ct);
}
