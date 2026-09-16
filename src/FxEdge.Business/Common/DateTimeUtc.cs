namespace FxEdge.Business.Common;

/// <summary>
/// Normalizes any DateTime to DateTimeKind.Utc consistently across the domain, without
/// silently shifting Unspecified-kind values (treated as already-UTC, since every
/// timestamp in this system is UTC by convention) while still converting genuinely
/// local/offset values correctly.
/// </summary>
internal static class DateTimeUtc
{
    public static DateTime Normalize(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
        _ => value.ToUniversalTime()
    };
}
