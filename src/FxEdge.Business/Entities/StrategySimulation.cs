using FxEdge.Contracts.Enums;

namespace FxEdge.Business.Entities;

/// <summary>
/// The result of applying a Strategy across every Entry with recorded EntryResult data
/// for a given Horizon: aggregate metrics plus the per-entry breakdown. Computed by
/// FxEdge.Business.Services.StrategySimulationService via IStrategySimulationEngine -
/// never user-entered, hence no Overwrite() (a new run is created each time instead).
/// </summary>
public sealed class StrategySimulation
{
    public Guid Id { get; private set; }
    public Guid StrategyId { get; private set; }
    public ResultHorizon Horizon { get; private set; }
    public DateTime RunAtUtc { get; private set; }

    public decimal ProfitAtr { get; private set; }
    public decimal LossAtr { get; private set; }
    public int SuccessfulEntries { get; private set; }
    public int FailedEntries { get; private set; }
    public decimal WinRatePercent { get; private set; }
    public decimal MaxDrawdownAtr { get; private set; }

    private readonly List<StrategySimulationEntryOutcome> _entryOutcomes = new();
    public IReadOnlyList<StrategySimulationEntryOutcome> EntryOutcomes => _entryOutcomes;

    // EF Core materialization constructor.
    private StrategySimulation()
    {
    }

    private StrategySimulation(
        Guid id, Guid strategyId, ResultHorizon horizon, DateTime runAtUtc,
        decimal profitAtr, decimal lossAtr, int successfulEntries, int failedEntries,
        decimal winRatePercent, decimal maxDrawdownAtr)
    {
        Id = id;
        StrategyId = strategyId;
        Horizon = horizon;
        RunAtUtc = runAtUtc;
        ProfitAtr = profitAtr;
        LossAtr = lossAtr;
        SuccessfulEntries = successfulEntries;
        FailedEntries = failedEntries;
        WinRatePercent = winRatePercent;
        MaxDrawdownAtr = maxDrawdownAtr;
    }

    public static StrategySimulation Create(
        Guid strategyId, ResultHorizon horizon, DateTime runAtUtc,
        decimal profitAtr, decimal lossAtr, int successfulEntries, int failedEntries,
        decimal winRatePercent, decimal maxDrawdownAtr,
        IReadOnlyList<StrategySimulationEntryOutcome> entryOutcomes)
    {
        var simulation = new StrategySimulation(
            Guid.NewGuid(), strategyId, horizon, runAtUtc,
            profitAtr, lossAtr, successfulEntries, failedEntries, winRatePercent, maxDrawdownAtr);

        simulation._entryOutcomes.AddRange(entryOutcomes);
        return simulation;
    }
}
