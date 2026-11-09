using TourGuide.DAL.Enums;

namespace TourGuide.PL.ViewModels;

// Guide "Requests & bookings" page (UI screen 3, FR-16).
public class GuideDashboardViewModel
{
    public List<GuideBookingItemViewModel> Bookings { get; set; } = new();

    // "Now" from TimeProvider, for the countdown notes.
    public DateTime NowUtc { get; set; }

    // Shown instead of the list when the guide cannot see requests (BR-01: not approved).
    public string? Notice { get; set; }

    public static IReadOnlyList<BookingStatus> ChipOrder => MyBookingsViewModel.ChipOrder;

    public int CountOf(BookingStatus status) => Bookings.Count(b => b.Status == status);
}
