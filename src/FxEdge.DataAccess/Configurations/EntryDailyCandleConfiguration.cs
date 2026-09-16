using FxEdge.Business.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FxEdge.DataAccess.Configurations;

/// <summary>
/// Maps EntryDailyCandle to the "EntryDailyCandles" table: up to 40 rows per Entry
/// (one per trading DayIndex), keyed by (EntryId, DayIndex). Independent table, not
/// FK-linked to Entries - like EntryResult, candles are recorded separately and later.
/// </summary>
public sealed class EntryDailyCandleConfiguration : IEntityTypeConfiguration<EntryDailyCandle>
{
    public void Configure(EntityTypeBuilder<EntryDailyCandle> builder)
    {
        builder.ToTable("EntryDailyCandles");

        builder.HasKey(c => new { c.EntryId, c.DayIndex });

        builder.Property(c => c.OpenAtr).IsRequired();
        builder.Property(c => c.HighAtr).IsRequired();
        builder.Property(c => c.LowAtr).IsRequired();
        builder.Property(c => c.CloseAtr).IsRequired();

        builder.Property(c => c.FirstExtreme)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();
    }
}
