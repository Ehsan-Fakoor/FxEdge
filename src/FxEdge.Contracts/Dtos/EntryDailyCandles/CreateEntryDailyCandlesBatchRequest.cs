namespace FxEdge.Contracts.Dtos.EntryDailyCandles;

/// <summary>
/// Request to record several days' candles in one call - entering up to 40 individually
/// would be impractical. All-or-nothing: if any candle is invalid or its DayIndex
/// conflicts (duplicated within this batch, or already recorded for this Entry), the
/// whole batch is rejected with no partial writes.
/// </summary>
public sealed record CreateEntryDailyCandlesBatchRequest(
    IReadOnlyList<CreateEntryDailyCandleRequest> Candles);
