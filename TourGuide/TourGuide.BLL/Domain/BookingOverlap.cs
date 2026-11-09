using TourGuide.DAL.Enums;

namespace TourGuide.BLL.Domain;

// BR-09: the overlap rule in one place for BLL (the DAL query in HasOverlapAsync uses the same formula).
public static class BookingOverlap
{
    // Only these statuses block a guide's time.
    public static bool Blocks(BookingStatus status) =>
        status is BookingStatus.Accepted or BookingStatus.Confirmed;

    // A.StartUtc < B.EndUtc && B.StartUtc < A.EndUtc — touching edges do not overlap.
    public static bool Overlaps(DateTime aStartUtc, DateTime aEndUtc, DateTime bStartUtc, DateTime bEndUtc) =>
        aStartUtc < bEndUtc && bStartUtc < aEndUtc;
}
