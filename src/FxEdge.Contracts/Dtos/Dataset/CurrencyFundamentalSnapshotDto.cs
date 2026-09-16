using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Dataset;

/// <summary>
/// One currency's complete fundamental snapshot as of a given UTC date: exactly 14
/// entries (one per FundamentalFeature, in fixed enum order), each independently
/// resolved point-in-time. This is an on-the-fly view, never persisted directly.
/// </summary>
public sealed record CurrencyFundamentalSnapshotDto(
    Currency Currency,
    DateTime AsOfUtc,
    IReadOnlyList<FeatureSnapshotDto> Features);
