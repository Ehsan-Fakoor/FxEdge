using FxEdge.Business.Entities;
using FxEdge.Contracts.Enums;

namespace FxEdge.Business.Abstractions;

public interface IEntryResultRepository
{
    Task AddAsync(EntryResult result, CancellationToken ct = default);

    Task<EntryResult?> GetAsync(Guid entryId, ResultHorizon horizon, CancellationToken ct = default);

    Task<IReadOnlyList<EntryResult>> QueryByEntryAsync(Guid entryId, CancellationToken ct = default);

    /// <summary>Every EntryResult recorded for a given Horizon, across all Entries - the input StrategySimulation needs.</summary>
    Task<IReadOnlyList<EntryResult>> QueryByHorizonAsync(ResultHorizon horizon, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
