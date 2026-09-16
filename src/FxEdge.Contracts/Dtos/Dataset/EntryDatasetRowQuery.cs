using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Dataset;

/// <summary>
/// Optional filters for bulk dataset row export. All filters are optional and combine
/// with AND semantics; leave a filter null to not constrain by it. Currency/date
/// filters apply to the Entry itself (matching EntryQuery); Horizon filters which
/// EntryResult(s) each matching Entry contributes rows for.
/// </summary>
public sealed record EntryDatasetRowQuery(
    Currency? BaseCurrency = null,
    Currency? QuoteCurrency = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    ResultHorizon? Horizon = null);
