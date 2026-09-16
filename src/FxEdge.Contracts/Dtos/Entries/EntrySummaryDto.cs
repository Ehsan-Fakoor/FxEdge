using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Entries;

/// <summary>
/// A lightweight view of an Entry for list screens - fetch the full EntryDto via
/// GetByIdAsync when the frozen fundamental/technical/formula detail is needed.
/// </summary>
public sealed record EntrySummaryDto(
    Guid Id,
    Currency BaseCurrency,
    Currency QuoteCurrency,
    string PairSymbol,
    DateTime EntryAtUtc,
    decimal EntryPrice,
    Direction Direction);
