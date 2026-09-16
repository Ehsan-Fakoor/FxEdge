using FxEdge.Business.Entities;
using FxEdge.Contracts.Enums;

namespace FxEdge.Business.Abstractions;

/// <summary>
/// One Entry's resolved outcome as computed by the engine, before it's wrapped into a
/// persisted StrategySimulationEntryOutcome.
/// </summary>
public sealed record EntrySimulationOutcome(Guid EntryId, SimulationOutcome Outcome, decimal? ResultAtr);

/// <summary>
/// The full computation IStrategySimulationEngine must produce for one simulation run.
/// StrategySimulationService turns this into a persisted StrategySimulation.
/// </summary>
public sealed record StrategySimulationComputation(
    decimal ProfitAtr,
    decimal LossAtr,
    int SuccessfulEntries,
    int FailedEntries,
    decimal WinRatePercent,
    decimal MaxDrawdownAtr,
    IReadOnlyList<EntrySimulationOutcome> EntryOutcomes);

/// <summary>
/// Computes a Strategy's simulated performance across a set of Entries' recorded
/// results for one Horizon. This is the computational core of Layer 3
/// (StrategySimulation) - structurally wired end-to-end (persistence, DTOs,
/// endpoints), but the algorithm itself is intentionally not yet implemented (see
/// FxEdge.Business.Services.NotYetDefinedStrategySimulationEngine): given only MFE and
/// MAE per Entry (no full price path), it is ambiguous (a) which threshold was hit
/// first when both the take-profit and stop-loss levels were reached within the same
/// horizon, and (b) what P/L an Entry that reached neither threshold should be counted
/// as closing at. Swapping this implementation is the only change needed once those
/// rules are defined.
/// </summary>
public interface IStrategySimulationEngine
{
    Task<StrategySimulationComputation> SimulateAsync(Strategy strategy, IReadOnlyList<EntryResult> entryResults, CancellationToken ct = default);
}
