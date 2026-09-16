using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.EntryDailyCandles;

/// <summary>
/// Request to record one trading day's candle. EntryId is supplied via the route, not
/// this body. DayIndex is 1-based (1 = the first trading day after the Entry), must be
/// within [1, 40] (the longest supported Horizon, TwoMonths, is 40 trading days).
/// </summary>
public sealed record CreateEntryDailyCandleRequest(
    int DayIndex,
    decimal OpenAtr,
    decimal HighAtr,
    decimal LowAtr,
    decimal CloseAtr,
    CandleExtremeOrder FirstExtreme);
