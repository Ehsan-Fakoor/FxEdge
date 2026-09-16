using FxEdge.Contracts.Dtos.Entries;

namespace FxEdge.Contracts.Services;

/// <summary>
/// Data-entry surface for manually marked chart entries. Entries are immutable once
/// created (there is intentionally no edit/delete in this phase) - Technical and
/// Fundamental data are historical snapshots and must not change after the fact.
/// </summary>
public interface IEntryService
{
    /// <summary>
    /// Records a new Entry. Resolves and freezes the 14-feature Fundamental Snapshot
    /// (Base + Quote + Differential) strictly from data announced at or before
    /// EntryAtUtc, stores the user-supplied Technical readings as-is, and computes the
    /// four Formula Features (currently a placeholder - see
    /// FxEdge.Business.Abstractions.IFormulaFeatureCalculator).
    /// </summary>
    Task<EntryDto> CreateAsync(CreateEntryRequest request, CancellationToken ct = default);

    Task<EntryDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<EntrySummaryDto>> QueryAsync(EntryQuery query, CancellationToken ct = default);
}
