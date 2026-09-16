namespace FxEdge.Contracts.Enums;

/// <summary>
/// Which of a single day's raw High/Low was touched first within that day. Raw/market
/// terms (not trade-direction-adjusted) - matches how EntryDailyCandle itself is
/// stored, so entering it never requires the user to think about Buy/Sell direction.
/// </summary>
public enum CandleExtremeOrder
{
    HighFirst,
    LowFirst
}
