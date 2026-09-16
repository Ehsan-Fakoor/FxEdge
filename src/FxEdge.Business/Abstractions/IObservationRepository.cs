using FxEdge.Business.Entities;
using FxEdge.Contracts.Enums;

namespace FxEdge.Business.Abstractions;

/// <summary>
/// Persistence port for FundamentalObservation, owned by the Business layer and
/// implemented by FxEdge.DataAccess. Business depends only on this abstraction, never
/// on EF Core directly.
/// </summary>
public interface IObservationRepository
{
    Task AddAsync(FundamentalObservation observation, CancellationToken ct = default);

    Task<FundamentalObservation?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Whether an observation already exists for this exact Currency+Feature+AnnouncementAtUtc (optionally excluding one Id, for edit checks).</summary>
    Task<bool> ExistsAsync(Currency currency, FundamentalFeature feature, DateTime announcementAtUtc, Guid? excludeId = null, CancellationToken ct = default);

    /// <summary>The observation, if any, for this exact Currency+Feature+AnnouncementAtUtc. Used to upsert derived features (e.g. RealInterestRate) in place.</summary>
    Task<FundamentalObservation?> GetExactAsync(Currency currency, FundamentalFeature feature, DateTime announcementAtUtc, CancellationToken ct = default);

    Task<IReadOnlyList<FundamentalObservation>> QueryAsync(Currency? currency, FundamentalFeature? feature, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct = default);

    /// <summary>The latest observation for Currency+Feature with AnnouncementAtUtc &lt;= asOfUtc (the point-in-time "current" reading).</summary>
    Task<FundamentalObservation?> GetLatestAsOfAsync(Currency currency, FundamentalFeature feature, DateTime asOfUtc, CancellationToken ct = default);

    /// <summary>The latest observation for Currency+Feature with AnnouncementAtUtc strictly before beforeUtc (used to resolve the "Previous" reading).</summary>
    Task<FundamentalObservation?> GetLatestBeforeAsync(Currency currency, FundamentalFeature feature, DateTime beforeUtc, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
