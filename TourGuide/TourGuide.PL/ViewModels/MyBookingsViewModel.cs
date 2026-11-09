using TourGuide.DAL.Enums;

namespace TourGuide.PL.ViewModels;

// "My Bookings" page (UI screen 6, FR-15).
public class MyBookingsViewModel
{
    public List<BookingItemViewModel> Bookings { get; set; } = new();

    // "Now" from TimeProvider, for the countdown notes.
    public DateTime NowUtc { get; set; }

    // Status chips in the order of the screen.
    public static IReadOnlyList<BookingStatus> ChipOrder { get; } = new[]
    {
        BookingStatus.Pending, BookingStatus.Accepted, BookingStatus.Confirmed,
        BookingStatus.Completed, BookingStatus.Rejected, BookingStatus.Cancelled,
    };

    public int CountOf(BookingStatus status) => Bookings.Count(b => b.Status == status);
}
