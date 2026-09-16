using FxEdge.Business.Abstractions;
using FxEdge.Business.Entities;
using FxEdge.Contracts.Exceptions;

namespace FxEdge.Business.Services;

/// <inheritdoc cref="IStrategySimulationEngine"/>
/// <remarks>
/// Placeholder for Layer 3's computational core. Everything around it (persistence,
/// DTOs, endpoints) is already fully wired; only this class needs to be replaced once
/// two rules are defined:
/// 1. Which threshold resolves first when both MFE reaches take-profit and MAE reaches
///    stop-loss within the same horizon (there is no ordering information - only the
///    two peak excursions are recorded).
/// 2. What P/L (in ATR) an Entry that reached neither threshold should be counted as
///    closing at - EntryResult currently records only MFE/MAE, not a closing value at
///    the horizon boundary.
/// </remarks>
public sealed class NotYetDefinedStrategySimulationEngine : IStrategySimulationEngine
{
    public Task<StrategySimulationComputation> SimulateAsync(
        Strategy strategy, IReadOnlyList<EntryResult> entryResults, CancellationToken ct = default) =>
        throw new FxEdgeNotImplementedException(
            "StrategySimulation's engine is not implemented yet: the tie-breaking rule (when both take-profit and " +
            "stop-loss are reached within the same horizon) and the horizon-end closing value (for Entries that " +
            "reach neither threshold) still need to be defined.");
}
