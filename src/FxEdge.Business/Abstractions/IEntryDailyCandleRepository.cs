using FxEdge.Business.Entities;

namespace FxEdge.Business.Abstractions;

public interface IEntryDailyCandleRepository
{
    Task AddAsync(EntryDailyCandle candle, CancellationToken ct = default);

    Task AddRangeAsync(IEnumerable<EntryDailyCandle> candles, CancellationToken ct = default);

    Task<EntryDailyCandle?> GetAsync(Guid entryId, int dayIndex, CancellationToken ct = default);

    Task<IReadOnlyList<EntryDailyCandle>> QueryByEntryAsync(Guid entryId, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
