using FxEdge.Contracts.Enums;

namespace FxEdge.Business.Entities;

/// <summary>
/// One Entry's resolved outcome within a StrategySimulation run. A pure data holder,
/// built exclusively by IStrategySimulationEngine/StrategySimulationService - hence
/// the internal constructor.
/// </summary>
public sealed class StrategySimulationEntryOutcome
{
    public Guid StrategySimulationId { get; private set; }
    public Guid EntryId { get; private set; }
    public SimulationOutcome Outcome { get; private set; }
    public decimal? ResultAtr { get; private set; }

    // EF Core materialization constructor.
    private StrategySimulationEntryOutcome()
    {
    }

    internal StrategySimulationEntryOutcome(Guid entryId, SimulationOutcome outcome, decimal? resultAtr)
    {
        EntryId = entryId;
        Outcome = outcome;
        ResultAtr = resultAtr;
    }
}
