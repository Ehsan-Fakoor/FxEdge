using FxEdge.Contracts.Dtos.EntryResults;
using FxEdge.Contracts.Enums;

namespace FxEdge.Contracts.Services;

/// <summary>
/// Data-entry surface for an Entry's per-horizon outcome (MFE/MAE), recorded manually
/// by the user once each horizon has elapsed.
/// </summary>
public interface IEntryResultService
{
    Task<EntryResultDto> CreateAsync(Guid entryId, CreateEntryResultRequest request, CancellationToken ct = default);

    Task<EntryResultDto> EditAsync(Guid entryId, ResultHorizon horizon, EditEntryResultRequest request, CancellationToken ct = default);

    Task<EntryResultDto?> GetAsync(Guid entryId, ResultHorizon horizon, CancellationToken ct = default);

    /// <summary>All results recorded so far for one Entry (up to 3 - one per Horizon).</summary>
    Task<IReadOnlyList<EntryResultDto>> QueryByEntryAsync(Guid entryId, CancellationToken ct = default);
}
