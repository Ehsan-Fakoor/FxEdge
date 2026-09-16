using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FxEdge.DataAccess.Configurations;

/// <summary>
/// SQLite does not round-trip DateTimeKind, so every DateTime read back would otherwise
/// come back as Unspecified. Every timestamp in this system is UTC by domain invariant
/// (entities normalize on construction), so these converters simply re-stamp
/// DateTimeKind.Utc on the way out of the database. Shared by every entity
/// configuration that has a UTC timestamp column.
/// </summary>
internal static class UtcDateTimeConverters
{
    public static readonly ValueConverter<DateTime, DateTime> NonNullable = new(
        toDb => toDb,
        fromDb => DateTime.SpecifyKind(fromDb, DateTimeKind.Utc));

    public static readonly ValueConverter<DateTime?, DateTime?> Nullable = new(
        toDb => toDb,
        fromDb => fromDb.HasValue ? DateTime.SpecifyKind(fromDb.Value, DateTimeKind.Utc) : fromDb);
}
