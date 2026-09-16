using FxEdge.Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FxEdge.DataAccess.Configurations;

/// <summary>
/// Maps EntryResult to the "EntryResults" table: at most 3 rows per Entry (one per
/// ResultHorizon), keyed by (EntryId, Horizon). Deliberately not FK-linked to Entries
/// with cascade delete configured elsewhere - kept as its own independent table since
/// results are recorded well after (and separately from) Entry creation.
/// </summary>
public sealed class EntryResultConfiguration : IEntityTypeConfiguration<EntryResult>
{
    public void Configure(EntityTypeBuilder<EntryResult> builder)
    {
        builder.ToTable("EntryResults");

        builder.HasKey(r => new { r.EntryId, r.Horizon });

        builder.Property(r => r.Horizon)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(r => r.MaximumFavorableExcursionAtr).IsRequired();
        builder.Property(r => r.MaximumAdverseExcursionAtr).IsRequired();
        builder.Property(r => r.ReturnAtHorizonEndAtr).IsRequired();

        builder.Property(r => r.FirstExtremeReached)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();
    }
}
