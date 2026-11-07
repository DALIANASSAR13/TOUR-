namespace TourGuide.PL.Extensions;

// Display only: converts stored UTC times to the guide's IANA time zone.
public static class TimeZoneHelper
{
    public static TimeZoneInfo Find(string? ianaId)
    {
        if (string.IsNullOrWhiteSpace(ianaId))
            return TimeZoneInfo.Utc;
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(ianaId);
        }
        catch (TimeZoneNotFoundException) when (TimeZoneInfo.TryConvertIanaIdToWindowsId(ianaId, out var windowsId))
        {
            return TimeZoneInfo.FindSystemTimeZoneById(windowsId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Utc;
        }
    }

    public static DateTime ToLocal(DateTime utc, TimeZoneInfo zone) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), zone);

    // "GMT+2", "GMT+5:30", "GMT" — .NET has no zone abbreviations such as EET.
    public static string OffsetLabel(DateTime utc, TimeZoneInfo zone)
    {
        var offset = zone.GetUtcOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc));
        if (offset == TimeSpan.Zero)
            return "GMT";
        var sign = offset < TimeSpan.Zero ? "-" : "+";
        var abs = offset.Duration();
        return abs.Minutes == 0 ? $"GMT{sign}{abs.Hours}" : $"GMT{sign}{abs.Hours}:{abs.Minutes:D2}";
    }
}
