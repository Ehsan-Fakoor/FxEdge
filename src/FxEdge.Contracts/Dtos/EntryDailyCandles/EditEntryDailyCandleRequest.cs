using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.EntryDailyCandles;

/// <summary>
/// Request to correct an already-recorded candle in place (e.g. a misread chart
/// value). EntryId and DayIndex are supplied via the route, not this body.
/// </summary>
public sealed record EditEntryDailyCandleRequest(
    decimal OpenAtr,
    decimal HighAtr,
    decimal LowAtr,
    decimal CloseAtr,
    CandleExtremeOrder FirstExtreme);
