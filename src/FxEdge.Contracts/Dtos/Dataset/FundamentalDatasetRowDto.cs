using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Dataset;

/// <summary>
/// One fully-resolved dataset row for a currency pair as of a given UTC date: the raw
/// per-currency fundamental snapshots (Base and Quote) plus the 14 derived pair-level
/// differentials. Built deterministically and strictly from data announced at or before
/// AsOfUtc, so it carries no look-ahead/leakage. This is the current scope of the
/// dataset row (Fundamental only) - Technical Features, Formula Features and
/// Result/Target fields are intentionally out of scope for this phase and will extend
/// this shape later without breaking it.
/// </summary>
public sealed record FundamentalDatasetRowDto(
    string PairSymbol,
    Currency BaseCurrency,
    Currency QuoteCurrency,
    DateTime AsOfUtc,
    CurrencyFundamentalSnapshotDto BaseSnapshot,
    CurrencyFundamentalSnapshotDto QuoteSnapshot,
    IReadOnlyList<PairFeatureDifferentialDto> Differentials);
