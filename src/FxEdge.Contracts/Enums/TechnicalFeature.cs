namespace FxEdge.Contracts.Enums;

/// <summary>
/// The fixed set of 7 technical features recorded manually by the user at Entry time.
/// Any of them may be null (not every Entry will have every reading available).
/// Display names live in FxEdge.Business.Catalog.TechnicalFeatureCatalog.
/// </summary>
public enum TechnicalFeature
{
    /// <summary>1. RSI</summary>
    Rsi = 1,

    /// <summary>2. ADX</summary>
    Adx = 2,

    /// <summary>3. ATR</summary>
    Atr = 3,

    /// <summary>4. Candle Body (Close - Open)</summary>
    CandleBody = 4,

    /// <summary>5. Candle Range (High - Low)</summary>
    CandleRange = 5,

    /// <summary>6. Upper Shadow</summary>
    UpperShadow = 6,

    /// <summary>7. Lower Shadow</summary>
    LowerShadow = 7
}
