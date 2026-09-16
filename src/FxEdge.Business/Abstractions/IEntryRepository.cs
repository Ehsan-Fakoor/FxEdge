using FxEdge.Business.Entities;
using FxEdge.Contracts.Enums;

namespace FxEdge.Business.Abstractions;

/// <summary>
/// Persistence port for Entry (and its owned FundamentalSnapshot/TechnicalSnapshot
/// children), owned by the Business layer and implemented by FxEdge.DataAccess.
/// </summary>
public interface IEntryRepository
{
    Task AddAsync(Entry entry, CancellationToken ct = default);

    /// <summary>Loads an Entry together with its full FundamentalSnapshot and TechnicalSnapshot.</summary>
    Task<Entry?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Entry>> QueryAsync(
        Currency? baseCurrency,
        Currency? quoteCurrency,
        DateTime? fromUtc,
        DateTime? toUtc,
        Direction? direction,
        CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
