using FxEdge.Business.Abstractions;
using FxEdge.Business.Entities;
using FxEdge.Contracts.Enums;
using Microsoft.EntityFrameworkCore;

namespace FxEdge.DataAccess.Repositories;

/// <inheritdoc cref="IObservationRepository"/>
public sealed class ObservationRepository : IObservationRepository
{
    private readonly FxEdgeDbContext _dbContext;

    public ObservationRepository(FxEdgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(FundamentalObservation observation, CancellationToken ct = default) =>
        await _dbContext.Observations.AddAsync(observation, ct);

    public Task<FundamentalObservation?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _dbContext.Observations.SingleOrDefaultAsync(o => o.Id == id, ct);

    public Task<bool> ExistsAsync(Currency currency, FundamentalFeature feature, DateTime announcementAtUtc, Guid? excludeId = null, CancellationToken ct = default)
    {
        var query = _dbContext.Observations.Where(o =>
            o.Currency == currency &&
            o.Feature == feature &&
            o.AnnouncementAtUtc == announcementAtUtc);

        if (excludeId.HasValue)
        {
            query = query.Where(o => o.Id != excludeId.Value);
        }

        return query.AnyAsync(ct);
    }

    public Task<FundamentalObservation?> GetExactAsync(Currency currency, FundamentalFeature feature, DateTime announcementAtUtc, CancellationToken ct = default) =>
        _dbContext.Observations.SingleOrDefaultAsync(o =>
            o.Currency == currency && o.Feature == feature && o.AnnouncementAtUtc == announcementAtUtc, ct);

    public async Task<IReadOnlyList<FundamentalObservation>> QueryAsync(Currency? currency, FundamentalFeature? feature, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct = default)
    {
        var query = _dbContext.Observations.AsQueryable();

        if (currency.HasValue)
        {
            query = query.Where(o => o.Currency == currency.Value);
        }

        if (feature.HasValue)
        {
            query = query.Where(o => o.Feature == feature.Value);
        }

        if (fromUtc.HasValue)
        {
            query = query.Where(o => o.AnnouncementAtUtc >= fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(o => o.AnnouncementAtUtc <= toUtc.Value);
        }

        return await query
            .OrderByDescending(o => o.AnnouncementAtUtc)
            .ToListAsync(ct);
    }

    public Task<FundamentalObservation?> GetLatestAsOfAsync(Currency currency, FundamentalFeature feature, DateTime asOfUtc, CancellationToken ct = default) =>
        _dbContext.Observations
            .Where(o => o.Currency == currency && o.Feature == feature && o.AnnouncementAtUtc <= asOfUtc)
            .OrderByDescending(o => o.AnnouncementAtUtc)
            .FirstOrDefaultAsync(ct);

    public Task<FundamentalObservation?> GetLatestBeforeAsync(Currency currency, FundamentalFeature feature, DateTime beforeUtc, CancellationToken ct = default) =>
        _dbContext.Observations
            .Where(o => o.Currency == currency && o.Feature == feature && o.AnnouncementAtUtc < beforeUtc)
            .OrderByDescending(o => o.AnnouncementAtUtc)
            .FirstOrDefaultAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        _dbContext.SaveChangesAsync(ct);
}
