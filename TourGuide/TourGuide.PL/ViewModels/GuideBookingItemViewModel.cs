using TourGuide.BLL.Domain;
using TourGuide.DAL.Enums;

namespace TourGuide.PL.ViewModels;

// One card on the guide's requests list (UI screen 3, FR-16). Reuses the display formatting of
// BookingItemViewModel and words the status notes for the guide.
public class GuideBookingItemViewModel : BookingItemViewModel
{
    public string TouristName { get; set; } = string.Empty;
    public string? TouristCountryCode { get; set; }
    public bool CanAccept { get; set; }
    public bool CanReject { get; set; }
    public int? OverlapsWithBookingId { get; set; }

    // BR-09 warning on Pending cards.
    public string? OverlapWarning => OverlapsWithBookingId is int id
        ? $"Overlaps booking #{RefOf(id)} — accepting will be refused."
        : null;

    public string TouristInitials =>
        string.Concat(TouristName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(p => p[0]));

    public string? TouristCountry => CountryName(TouristCountryCode);

    public override string StatusNote(DateTime nowUtc) => Status switch
    {
        BookingStatus.Pending => $"New request. Answer within {HoursLeft(AutoCancelAtUtc, nowUtc)} h or it is cancelled automatically.",
        BookingStatus.Accepted => $"You accepted. The tourist must pay before {DeadlineText}.", // BR-08
        BookingStatus.Confirmed => $"Paid. {StartsIn(nowUtc)}",
        BookingStatus.Completed when ReviewRating is int r => $"Trip finished. Rated {r} of {BookingRules.MaxRating}.",
        BookingStatus.Completed => "Trip finished.",
        BookingStatus.Rejected => "You declined this request.",
        BookingStatus.Cancelled => CancellationReason switch
        {
            DAL.Enums.CancellationReason.WithdrawnByTourist => "The tourist withdrew this request.",
            DAL.Enums.CancellationReason.NoGuideResponse => $"Not answered within {BookingRules.PendingExpiryHours} h.",
            DAL.Enums.CancellationReason.NotPaid => "Not paid before the payment deadline.",
            _ => "Cancelled.",
        },
        _ => string.Empty,
    };
}
