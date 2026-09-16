using FxEdge.Business.Abstractions;
using FxEdge.Business.Entities;
using FxEdge.Contracts.Dtos.Strategies;
using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;
using FxEdge.Contracts.Services;

namespace FxEdge.Business.Services;

/// <inheritdoc cref="IStrategySimulationService"/>
public sealed class StrategySimulationService : IStrategySimulationService
{
    private readonly IStrategyRepository _strategyRepository;
    private readonly IEntryResultRepository _entryResultRepository;
    private readonly IStrategySimulationRepository _simulationRepository;
    private readonly IStrategySimulationEngine _engine;

    public StrategySimulationService(
        IStrategyRepository strategyRepository,
        IEntryResultRepository entryResultRepository,
        IStrategySimulationRepository simulationRepository,
        IStrategySimulationEngine engine)
    {
        _strategyRepository = strategyRepository;
        _entryResultRepository = entryResultRepository;
        _simulationRepository = simulationRepository;
        _engine = engine;
    }

    public async Task<StrategySimulationDto> RunAsync(Guid strategyId, ResultHorizon horizon, CancellationToken ct = default)
    {
        var strategy = await _strategyRepository.GetByIdAsync(strategyId, ct)
            ?? throw new FxEdgeNotFoundException($"Strategy '{strategyId}' was not found.");

        var entryResults = await _entryResultRepository.QueryByHorizonAsync(horizon, ct);

        // Everything past this point is delegated to IStrategySimulationEngine, which
        // is currently a structural placeholder (NotYetDefinedStrategySimulationEngine)
        // that throws FxEdgeNotImplementedException.
        var computation = await _engine.SimulateAsync(strategy, entryResults, ct);

        var entryOutcomes = computation.EntryOutcomes
            .Select(o => new StrategySimulationEntryOutcome(o.EntryId, o.Outcome, o.ResultAtr))
            .ToList();

        var simulation = StrategySimulation.Create(
            strategyId, horizon, DateTime.UtcNow,
            computation.ProfitAtr, computation.LossAtr, computation.SuccessfulEntries, computation.FailedEntries,
            computation.WinRatePercent, computation.MaxDrawdownAtr, entryOutcomes);

        await _simulationRepository.AddAsync(simulation, ct);
        await _simulationRepository.SaveChangesAsync(ct);

        return ToDto(simulation);
    }

    public async Task<StrategySimulationDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var simulation = await _simulationRepository.GetByIdAsync(id, ct);
        return simulation is null ? null : ToDto(simulation);
    }

    public async Task<IReadOnlyList<StrategySimulationDto>> QueryByStrategyAsync(Guid strategyId, CancellationToken ct = default)
    {
        var simulations = await _simulationRepository.QueryByStrategyAsync(strategyId, ct);
        return simulations.Select(ToDto).ToList();
    }

    private static StrategySimulationDto ToDto(StrategySimulation simulation) => new(
        simulation.Id,
        simulation.StrategyId,
        simulation.Horizon,
        simulation.RunAtUtc,
        simulation.ProfitAtr,
        simulation.LossAtr,
        simulation.SuccessfulEntries,
        simulation.FailedEntries,
        simulation.WinRatePercent,
        simulation.MaxDrawdownAtr,
        simulation.EntryOutcomes
            .Select(o => new StrategySimulationEntryOutcomeDto(o.EntryId, o.Outcome, o.ResultAtr))
            .ToList());
}
