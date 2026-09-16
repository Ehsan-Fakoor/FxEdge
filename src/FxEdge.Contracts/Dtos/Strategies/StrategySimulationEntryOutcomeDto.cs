using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Dtos.Strategies;

/// <summary>
/// How one Entry resolved within a simulation run, and what it contributed to the
/// result in ATR. ResultAtr is null only if the computation genuinely could not
/// resolve an outcome for this Entry (e.g. missing EntryResult data for the horizon).
/// </summary>
public sealed record StrategySimulationEntryOutcomeDto(
    Guid EntryId,
    SimulationOutcome Outcome,
    decimal? ResultAtr);
