using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Observations;

/// <summary>
/// Optional filters for listing observations. All filters are optional and combine
/// with AND semantics; leave a filter null to not constrain by it.
/// </summary>
public sealed record ObservationQuery(
    Currency? Currency = null,
    FundamentalFeature? Feature = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null);
