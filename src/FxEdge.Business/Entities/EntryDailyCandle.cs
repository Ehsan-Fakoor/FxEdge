using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;

namespace FxEdge.Business.Entities;

/// <summary>
/// One trading day's OHLC candle after an Entry, in raw price-direction terms
/// normalized to ATR distance from the Entry price (not trade-direction-adjusted -
/// unlike MFE/MAE - so entry matches reading a chart directly). Optional/supplementary
/// alongside EntryResult, never a replacement for it. Supports Overwrite() for
/// correcting a misread value, like EntryResult and FundamentalObservation.
/// </summary>
public sealed class EntryDailyCandle
{
    /// <summary>The longest supported Horizon (TwoMonths) spans 40 trading days.</summary>
    public const int MaxDayIndex = 40;

    public Guid EntryId { get; private set; }

    /// <summary>1-based trading day index after the Entry (1 = the first day after).</summary>
    public int DayIndex { get; private set; }

    public decimal OpenAtr { get; private set; }
    public decimal HighAtr { get; private set; }
    public decimal LowAtr { get; private set; }
    public decimal CloseAtr { get; private set; }

    /// <summary>Whether this day's High or Low was touched first, in raw market terms.</summary>
    public CandleExtremeOrder FirstExtreme { get; private set; }

    // EF Core materialization constructor.
    private EntryDailyCandle()
    {
    }

    private EntryDailyCandle(Guid entryId, int dayIndex, decimal openAtr, decimal highAtr, decimal lowAtr, decimal closeAtr, CandleExtremeOrder firstExtreme)
    {
        EntryId = entryId;
        DayIndex = dayIndex;
        OpenAtr = openAtr;
        HighAtr = highAtr;
        LowAtr = lowAtr;
        CloseAtr = closeAtr;
        FirstExtreme = firstExtreme;
    }

    public static EntryDailyCandle Create(Guid entryId, int dayIndex, decimal openAtr, decimal highAtr, decimal lowAtr, decimal closeAtr, CandleExtremeOrder firstExtreme)
    {
        Validate(dayIndex, openAtr, highAtr, lowAtr, closeAtr, firstExtreme);
        return new EntryDailyCandle(entryId, dayIndex, openAtr, highAtr, lowAtr, closeAtr, firstExtreme);
    }

    public void Overwrite(decimal openAtr, decimal highAtr, decimal lowAtr, decimal closeAtr, CandleExtremeOrder firstExtreme)
    {
        Validate(DayIndex, openAtr, highAtr, lowAtr, closeAtr, firstExtreme);
        OpenAtr = openAtr;
        HighAtr = highAtr;
        LowAtr = lowAtr;
        CloseAtr = closeAtr;
        FirstExtreme = firstExtreme;
    }

    private static void Validate(int dayIndex, decimal openAtr, decimal highAtr, decimal lowAtr, decimal closeAtr, CandleExtremeOrder firstExtreme)
    {
        if (dayIndex is < 1 or > MaxDayIndex)
        {
            throw new FxEdgeValidationException($"DayIndex must be between 1 and {MaxDayIndex} (got {dayIndex}).");
        }

        if (!Enum.IsDefined(firstExtreme))
        {
            throw new FxEdgeValidationException($"'{firstExtreme}' is not a recognized candle extreme order.");
        }

        // Standard OHLC sanity: High is the day's maximum, Low is its minimum.
        if (highAtr < lowAtr)
        {
            throw new FxEdgeValidationException($"HighAtr ({highAtr}) cannot be less than LowAtr ({lowAtr}).");
        }

        if (highAtr < openAtr || highAtr < closeAtr)
        {
            throw new FxEdgeValidationException("HighAtr must be greater than or equal to both OpenAtr and CloseAtr.");
        }

        if (lowAtr > openAtr || lowAtr > closeAtr)
        {
            throw new FxEdgeValidationException("LowAtr must be less than or equal to both OpenAtr and CloseAtr.");
        }
    }
}
