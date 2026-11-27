using System.Globalization;
using TourGuide.BLL.Domain;
using TourGuide.DAL.Enums;
using TourGuide.PL.Extensions;

namespace TourGuide.PL.ViewModels;

// One card on "My Bookings" (UI screen 6); the guide's list (screen 3) extends it.
// Settable fields come from BookingDto by AutoMapper;
// the get-only properties only format them for display (English, guide's time zone).
public class BookingItemViewModel
{
    protected static readonly CultureInfo En = CultureInfo.InvariantCulture;

    public int Id { get; set; }
    public int GuideProfileId { get; set; }
    public string GuideName { get; set; } = string.Empty;
    public string? GuideCity { get; set; }
    public string? GuideCountryCode { get; set; }
    public string? GuideTimeZoneId { get; set; }
    public BookingType BookingType { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public int? Hours { get; set; }
    public int? Days { get; set; }
    public int BillableHours { get; set; }
    public decimal PricePerHourAtRequest { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? PaymentDeadlineUtc { get; set; }
    public CancellationReason? CancellationReason { get; set; }
    public bool IsPaid { get; set; }
    public int? ReviewRating { get; set; }
    public string? ReviewComment { get; set; }
    public DateTime? AutoCancelAtUtc { get; set; }
    public bool CanPay { get; set; }
    public bool CanWithdraw { get; set; }
    public bool CanReview { get; set; }

    private TimeZoneInfo? _zone;
    protected TimeZoneInfo Zone => _zone ??= TimeZoneHelper.Find(GuideTimeZoneId);

    public string Reference => RefOf(Id);

    // "TG-0042" — one format for every booking reference on screen.
    public static string RefOf(int id) => $"TG-{id:D4}";

    public string SentText => "sent " + TimeZoneHelper.ToLocal(CreatedAtUtc, Zone).ToString("MMM d", En);

    public string Initials =>
        string.Concat(GuideName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(p => p[0]));

    public string Location
    {
        get
        {
            var country = CountryName(GuideCountryCode);
            return string.Join(", ", new[] { GuideCity, country }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }
    }

    // "Fri, Nov 13 · 10:00–16:00 (GMT+2) · 6 hours" or "Nov 20 – Nov 22 · 3 days"
    public string WhenText
    {
        get
        {
            var start = TimeZoneHelper.ToLocal(StartUtc, Zone);
            var end = TimeZoneHelper.ToLocal(EndUtc, Zone);
            if (BookingType == BookingType.Hourly)
                return $"{start.ToString("ddd, MMM d", En)} · {start:HH:mm}–{end:HH:mm} ({TimeZoneHelper.OffsetLabel(StartUtc, Zone)}) · {Hours} hours";
            var lastDay = end.AddDays(-1);
            return $"{start.ToString("MMM d", En)} – {lastDay.ToString("MMM d", En)} · {Days} days";
        }
    }

    // "$150.00" + "6 h × $25" / "3 days × 8 h × $22" / "paid" / "not charged"
    public string TotalText => Money(TotalPrice, alwaysCents: true);

    // Built from the booking's own stored numbers, so it always matches TotalPrice.
    public string PriceDetail => Status switch
    {
        _ when IsPaid => "paid",
        BookingStatus.Rejected or BookingStatus.Cancelled => "not charged",
        _ when BookingType == BookingType.Hourly => $"{Hours} h × {Money(PricePerHourAtRequest)}",
        _ when Days is > 0 => $"{Days} days × {BillableHours / Days} h × {Money(PricePerHourAtRequest)}",
        _ => $"{BillableHours} h × {Money(PricePerHourAtRequest)}",
    };

    public string StatusCss => Status.ToString().ToLowerInvariant();

    public bool IsWarningNote => Status is BookingStatus.Pending or BookingStatus.Accepted;

    // The line under the date, by status (UI screen 6).
    public virtual string StatusNote(DateTime nowUtc) => Status switch
    {
        BookingStatus.Pending => $"Waiting for the guide. Auto-cancels if not answered within {HoursLeft(AutoCancelAtUtc, nowUtc)} h.",
        BookingStatus.Accepted => $"Guide accepted. Pay before {DeadlineText} or the booking is cancelled.",
        BookingStatus.Confirmed => $"Paid. {StartsIn(nowUtc)}",
        BookingStatus.Completed when ReviewRating is null => "Trip finished. Share how it went.",
        BookingStatus.Completed when !string.IsNullOrWhiteSpace(ReviewComment) => $"“{ReviewComment}”",
        BookingStatus.Completed => "Trip finished. Thank you for your review.",
        BookingStatus.Rejected => "The guide declined this request.",
        BookingStatus.Cancelled => CancellationReason switch
        {
            DAL.Enums.CancellationReason.WithdrawnByTourist => "You withdrew this booking.",
            DAL.Enums.CancellationReason.NoGuideResponse => "The guide did not answer in time.",
            DAL.Enums.CancellationReason.NotPaid => "Not paid before the payment deadline.",
            _ => "Cancelled.",
        },
        _ => string.Empty,
    };

    public string Stars => ReviewRating is int r ? new string('★', r) + new string('☆', BookingRules.MaxRating - r) : string.Empty;

    protected string DeadlineText
    {
        get
        {
            if (PaymentDeadlineUtc is not DateTime deadline)
                return "the deadline";
            var local = TimeZoneHelper.ToLocal(deadline, Zone);
            return $"{local.ToString("MMM d, HH:mm", En)} ({TimeZoneHelper.OffsetLabel(deadline, Zone)})";
        }
    }

    protected string StartsIn(DateTime nowUtc)
    {
        var today = DateOnly.FromDateTime(TimeZoneHelper.ToLocal(nowUtc, Zone));
        var startDay = DateOnly.FromDateTime(TimeZoneHelper.ToLocal(StartUtc, Zone));
        var days = startDay.DayNumber - today.DayNumber;
        return days switch
        {
            <= 0 => "Your trip starts today.",
            1 => "Your trip starts tomorrow.",
            _ => $"Your trip starts in {days} days.",
        };
    }

    protected static int HoursLeft(DateTime? untilUtc, DateTime nowUtc) =>
        untilUtc is DateTime until ? Math.Max(0, (int)Math.Ceiling((until - nowUtc).TotalHours)) : 0;

    // One money format for the page: "$1,250.00" for totals, "$25" / "$22.50" for rates. USD only.
    protected static string Money(decimal value, bool alwaysCents = false) =>
        "$" + value.ToString(alwaysCents || value % 1 != 0 ? "#,0.00" : "#,0", En);

    protected static string? CountryName(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;
        try { return new RegionInfo(code.Trim()).EnglishName; }
        catch (ArgumentException) { return code; }
    }
}
