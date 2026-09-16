using FxEdge.Contracts.Dtos.EntryDailyCandles;

namespace FxEdge.Contracts.Services;

/// <summary>
/// Data-entry surface for an Entry's optional, opt-in daily candle series (raw OHLC,
/// ATR-normalized). Supplements EntryResult for more precise StrategySimulation of
/// multi-entry (grid/pyramiding) strategies - EntryResult itself is untouched by this.
/// </summary>
public interface IEntryDailyCandleService
{
    Task<EntryDailyCandleDto> CreateAsync(Guid entryId, CreateEntryDailyCandleRequest request, CancellationToken ct = default);

    /// <summary>Records several days at once. All-or-nothing - see CreateEntryDailyCandlesBatchRequest.</summary>
    Task<IReadOnlyList<EntryDailyCandleDto>> CreateBatchAsync(Guid entryId, CreateEntryDailyCandlesBatchRequest request, CancellationToken ct = default);

    Task<EntryDailyCandleDto> EditAsync(Guid entryId, int dayIndex, EditEntryDailyCandleRequest request, CancellationToken ct = default);

    Task<EntryDailyCandleDto?> GetAsync(Guid entryId, int dayIndex, CancellationToken ct = default);

    /// <summary>All candles recorded so far for one Entry, ordered by DayIndex.</summary>
    Task<IReadOnlyList<EntryDailyCandleDto>> QueryByEntryAsync(Guid entryId, CancellationToken ct = default);
}
