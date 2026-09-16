using FxEdge.Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FxEdge.DataAccess.Configurations;

/// <summary>
/// Maps EntryTechnicalFeatureSnapshot to the "EntryTechnicalFeatureSnapshots" table:
/// exactly 7 rows per Entry (one per TechnicalFeature), keyed by (EntryId, Feature).
/// </summary>
public sealed class EntryTechnicalFeatureSnapshotConfiguration : IEntityTypeConfiguration<EntryTechnicalFeatureSnapshot>
{
    public void Configure(EntityTypeBuilder<EntryTechnicalFeatureSnapshot> builder)
    {
        builder.ToTable("EntryTechnicalFeatureSnapshots");

        builder.HasKey(s => new { s.EntryId, s.Feature });

        builder.Property(s => s.Feature)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
    }
}
