using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Entries;

/// <summary>
/// Optional filters for listing entries. All filters are optional and combine with AND
/// semantics; leave a filter null to not constrain by it.
/// </summary>
public sealed record EntryQuery(
    Currency? BaseCurrency = null,
    Currency? QuoteCurrency = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    Direction? Direction = null);
