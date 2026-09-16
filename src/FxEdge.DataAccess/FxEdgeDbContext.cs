using FxEdge.Business.Entities;
using FxEdge.DataAccess.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FxEdge.DataAccess;

/// <summary>
/// The single EF Core DbContext for FxEdge's local SQLite database. Only entities are
/// mapped here (FundamentalObservation, Entry and its two owned snapshot collections) -
/// CurrencyPair and every Dataset DTO are computed on the fly and never persisted.
/// </summary>
public sealed class FxEdgeDbContext : DbContext
{
    public FxEdgeDbContext(DbContextOptions<FxEdgeDbContext> options) : base(options)
    {
    }

    public DbSet<FundamentalObservation> Observations => Set<FundamentalObservation>();

    public DbSet<Entry> Entries => Set<Entry>();

    public DbSet<EntryResult> EntryResults => Set<EntryResult>();

    public DbSet<EntryDailyCandle> EntryDailyCandles => Set<EntryDailyCandle>();

    public DbSet<Strategy> Strategies => Set<Strategy>();

    public DbSet<StrategySimulation> StrategySimulations => Set<StrategySimulation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FundamentalObservationConfiguration());
        modelBuilder.ApplyConfiguration(new EntryConfiguration());
        modelBuilder.ApplyConfiguration(new EntryFundamentalFeatureSnapshotConfiguration());
        modelBuilder.ApplyConfiguration(new EntryTechnicalFeatureSnapshotConfiguration());
        modelBuilder.ApplyConfiguration(new EntryResultConfiguration());
        modelBuilder.ApplyConfiguration(new EntryDailyCandleConfiguration());
        modelBuilder.ApplyConfiguration(new StrategyConfiguration());
        modelBuilder.ApplyConfiguration(new StrategySimulationConfiguration());
        modelBuilder.ApplyConfiguration(new StrategySimulationEntryOutcomeConfiguration());
    }
}
