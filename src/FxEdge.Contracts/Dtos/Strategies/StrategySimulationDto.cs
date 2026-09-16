using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Strategies;

/// <summary>
/// The result of applying a Strategy across every Entry that has recorded EntryResult
/// data for a given Horizon: aggregate backtest-style metrics plus the per-entry
/// breakdown that produced them. Computed by the app (see
/// FxEdge.Business.Abstractions.IStrategySimulationEngine) - not user-entered.
/// </summary>
public sealed record StrategySimulationDto(
    Guid Id,
    Guid StrategyId,
    ResultHorizon Horizon,
    DateTime RunAtUtc,
    decimal ProfitAtr,
    decimal LossAtr,
    int SuccessfulEntries,
    int FailedEntries,
    decimal WinRatePercent,
    decimal MaxDrawdownAtr,
    IReadOnlyList<StrategySimulationEntryOutcomeDto> EntryOutcomes);
