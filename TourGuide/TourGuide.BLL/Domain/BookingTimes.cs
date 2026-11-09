using TourGuide.BLL.DTOs.Booking;
using TourGuide.DAL.Enums;

namespace TourGuide.BLL.Domain;

// Guide-local trip times -> UTC (Class Specs §5 StartUtc / EndUtc). Used by the validator and BookingService,
// so the conversion lives in one place.
public static class BookingTimes
{
    public static TimeZoneInfo FindZone(string ianaId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(ianaId);
        }
        catch (TimeZoneNotFoundException) when (TimeZoneInfo.TryConvertIanaIdToWindowsId(ianaId, out var windowsId))
        {
            return TimeZoneInfo.FindSystemTimeZoneById(windowsId);
        }
    }

    // Null when the request has no start yet, or an hourly start time does not exist (clock moved forward).
    public static (DateTime StartUtc, DateTime EndUtc)? Resolve(CreateBookingRequest r, TimeZoneInfo zone)
    {
        if (r.StartDate is not { } date)
            return null;

        if (r.BookingType == BookingType.MultiDay)
        {
            if (r.Days is not { } days)
                return null;
            return (StartOfDayUtc(date, zone), StartOfDayUtc(date.AddDays(days), zone));
        }

        if (r.StartTime is not { } time || r.Hours is not { } hours)
            return null;
        if (!TryLocalToUtc(date.ToDateTime(time), zone, out var startUtc))
            return null;
        return (startUtc, startUtc.AddHours(hours));
    }

    // False when the local time is skipped by a clock change. A repeated time takes its first occurrence.
    public static bool TryLocalToUtc(DateTime local, TimeZoneInfo zone, out DateTime utc)
    {
        local = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
        utc = default;
        if (zone.IsInvalidTime(local))
            return false;

        var offset = zone.IsAmbiguousTime(local)
            ? zone.GetAmbiguousTimeOffsets(local).Max()
            : zone.GetUtcOffset(local);
        utc = DateTime.SpecifyKind(local - offset, DateTimeKind.Utc);
        return true;
    }

    // 00:00 local, or the first valid minute of the day when midnight is skipped (e.g. Africa/Cairo in April).
    private static DateTime StartOfDayUtc(DateOnly date, TimeZoneInfo zone)
    {
        var local = date.ToDateTime(TimeOnly.MinValue);
        for (var i = 0; i < 24 * 4; i++, local = local.AddMinutes(15))
            if (TryLocalToUtc(local, zone, out var utc))
                return utc;
        throw new InvalidOperationException($"No valid time on {date} in {zone.Id}.");
    }
}
