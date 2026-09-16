using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.EntryDailyCandles;

/// <summary>
/// One trading day's OHLC candle after an Entry, in raw price-direction terms
/// normalized to ATR distance from the Entry price: e.g. HighAtr = (market High -
/// EntryPrice) / ATR. Raw, not trade-direction-adjusted - unlike MFE/MAE
/// ("Favorable"/"Adverse"), so entering it matches reading a chart directly, with no
/// mental Buy/Sell adjustment required.
///
/// Optional/supplementary alongside EntryResult, not a replacement: EntryResult stays
/// the quick, always-available MFE/MAE/closing-return summary for a whole Horizon;
/// EntryDailyCandle is the more laborious, opt-in, day-by-day detail that lets
/// StrategySimulation simulate a grid/pyramiding strategy's multiple entry levels
/// precisely instead of only the original entry's endpoint statistics.
///
/// Horizon-agnostic: the 3 Horizons are nested trading-day windows (1W=days 1-5,
/// 1M=days 1-20, 2M=days 1-40), so a single 40-day candle series covers all of them -
/// no separate candle sets per Horizon.
/// </summary>
public sealed record EntryDailyCandleDto(
    Guid EntryId,
    int DayIndex,
    decimal OpenAtr,
    decimal HighAtr,
    decimal LowAtr,
    decimal CloseAtr,
    CandleExtremeOrder FirstExtreme);
