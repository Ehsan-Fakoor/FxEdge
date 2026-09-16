using FxEdge.Contracts.Dtos.Dataset;

namespace FxEdge.Contracts.Services;

/// <summary>
/// Read-only surface for the combined Entry+EntryResult dataset rows (features + label,
/// one row per Entry per Horizon that has a recorded result) - the ML-ready export of
/// everything captured so far. Purely a composition of IEntryService/IEntryResultService;
/// creates nothing new, computes nothing new.
/// </summary>
public interface IEntryDatasetService
{
    /// <summary>Every matching (Entry, Horizon) row, across all Entries.</summary>
    Task<IReadOnlyList<EntryDatasetRowDto>> QueryAsync(EntryDatasetRowQuery query, CancellationToken ct = default);

    /// <summary>All rows for one specific Entry (one per Horizon it has a recorded result for).</summary>
    Task<IReadOnlyList<EntryDatasetRowDto>> QueryByEntryAsync(Guid entryId, CancellationToken ct = default);
}
