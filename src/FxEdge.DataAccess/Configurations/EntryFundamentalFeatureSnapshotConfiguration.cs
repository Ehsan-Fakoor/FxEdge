using FxEdge.Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FxEdge.DataAccess.Configurations;

/// <summary>
/// Maps EntryFundamentalFeatureSnapshot to the "EntryFundamentalFeatureSnapshots"
/// table: exactly 14 rows per Entry (one per FundamentalFeature), keyed by
/// (EntryId, Feature).
/// </summary>
public sealed class EntryFundamentalFeatureSnapshotConfiguration : IEntityTypeConfiguration<EntryFundamentalFeatureSnapshot>
{
    public void Configure(EntityTypeBuilder<EntryFundamentalFeatureSnapshot> builder)
    {
        builder.ToTable("EntryFundamentalFeatureSnapshots");

        builder.HasKey(s => new { s.EntryId, s.Feature });

        builder.Property(s => s.Feature)
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(s => s.BaseAnnouncementAtUtc)
            .HasConversion(UtcDateTimeConverters.Nullable);

        builder.Property(s => s.QuoteAnnouncementAtUtc)
            .HasConversion(UtcDateTimeConverters.Nullable);
    }
}
