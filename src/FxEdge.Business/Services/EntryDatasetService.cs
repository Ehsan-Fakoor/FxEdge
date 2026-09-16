using FxEdge.Contracts.Dtos.Dataset;
using FxEdge.Contracts.Dtos.Entries;
using FxEdge.Contracts.Enums;
using FxEdge.Contracts.Exceptions;
using FxEdge.Contracts.Services;

namespace FxEdge.Business.Services;

/// <inheritdoc cref="IEntryDatasetService"/>
/// <remarks>
/// Purely a composition of IEntryService and IEntryResultService - no repository
/// dependencies, no DTO-mapping logic of its own. Necessarily N+1-ish (one
/// GetByIdAsync and one QueryByEntryAsync per matching Entry), matching the
/// personal-scale-app tradeoff already accepted elsewhere (e.g.
/// DatasetService.GetAllPairsDatasetRowsAsync loops the 28 pairs individually too).
/// </remarks>
public sealed class EntryDatasetService : IEntryDatasetService
{
    private readonly IEntryService _entryService;
    private readonly IEntryResultService _entryResultService;

    public EntryDatasetService(IEntryService entryService, IEntryResultService entryResultService)
    {
        _entryService = entryService;
        _entryResultService = entryResultService;
    }

    public async Task<IReadOnlyList<EntryDatasetRowDto>> QueryAsync(EntryDatasetRowQuery query, CancellationToken ct = default)
    {
        var summaries = await _entryService.QueryAsync(
            new EntryQuery(query.BaseCurrency, query.QuoteCurrency, query.FromUtc, query.ToUtc, Direction: null), ct);

        var rows = new List<EntryDatasetRowDto>();
        foreach (var summary in summaries)
        {
            var entry = await _entryService.GetByIdAsync(summary.Id, ct);
            if (entry is null)
            {
                continue; // defensive - shouldn't happen between the two calls above.
            }

            rows.AddRange(await BuildRowsForEntryAsync(entry, query.Horizon, ct));
        }

        return rows;
    }

    public async Task<IReadOnlyList<EntryDatasetRowDto>> QueryByEntryAsync(Guid entryId, CancellationToken ct = default)
    {
        var entry = await _entryService.GetByIdAsync(entryId, ct)
            ?? throw new FxEdgeNotFoundException($"Entry '{entryId}' was not found.");

        return await BuildRowsForEntryAsync(entry, horizonFilter: null, ct);
    }

    private async Task<IReadOnlyList<EntryDatasetRowDto>> BuildRowsForEntryAsync(EntryDto entry, ResultHorizon? horizonFilter, CancellationToken ct)
    {
        var results = await _entryResultService.QueryByEntryAsync(entry.Id, ct);

        var relevantResults = horizonFilter.HasValue
            ? results.Where(r => r.Horizon == horizonFilter.Value)
            : results;

        return relevantResults
            .Select(result => new EntryDatasetRowDto(entry.Id, result.Horizon, entry, result))
            .ToList();
    }
}
