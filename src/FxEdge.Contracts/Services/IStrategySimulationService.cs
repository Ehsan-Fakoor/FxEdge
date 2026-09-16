using FxEdge.Contracts.Dtos.Strategies;
using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Services;

/// <summary>
/// Triggers and retrieves computed Strategy simulations (Layer 3). RunAsync applies a
/// Strategy across every Entry that has recorded EntryResult data for the given
/// Horizon and persists the outcome. As of this phase, the underlying computation
/// (FxEdge.Business.Abstractions.IStrategySimulationEngine) is a structural
/// placeholder - RunAsync currently throws FxEdgeNotImplementedException until the
/// exact tie-breaking and horizon-end-closing rules are defined.
/// </summary>
public interface IStrategySimulationService
{
    Task<StrategySimulationDto> RunAsync(Guid strategyId, ResultHorizon horizon, CancellationToken ct = default);

    Task<StrategySimulationDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Every past simulation run recorded for a Strategy, most recent first.</summary>
    Task<IReadOnlyList<StrategySimulationDto>> QueryByStrategyAsync(Guid strategyId, CancellationToken ct = default);
}
