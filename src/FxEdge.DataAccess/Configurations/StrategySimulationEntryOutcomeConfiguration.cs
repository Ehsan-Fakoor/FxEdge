using FxEdge.Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FxEdge.DataAccess.Configurations;

/// <summary>
/// Maps StrategySimulationEntryOutcome to the "StrategySimulationEntryOutcomes" table,
/// keyed by (StrategySimulationId, EntryId).
/// </summary>
public sealed class StrategySimulationEntryOutcomeConfiguration : IEntityTypeConfiguration<StrategySimulationEntryOutcome>
{
    public void Configure(EntityTypeBuilder<StrategySimulationEntryOutcome> builder)
    {
        builder.ToTable("StrategySimulationEntryOutcomes");

        builder.HasKey(o => new { o.StrategySimulationId, o.EntryId });

        builder.Property(o => o.Outcome)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(o => o.ResultAtr);
    }
}
