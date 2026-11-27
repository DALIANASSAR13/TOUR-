using TourGuide.DAL.Enums;

namespace TourGuide.BLL.Domain;

// A booking whose time ran out and that now belongs to the System job (FR-17):
// Pending without an answer (BR-06) or Accepted and not paid by the deadline (BR-08).
// Users cannot change it any more; the job cancels it with NoGuideResponse / NotPaid.
public static class BookingExpiry
{
    public static bool IsExpired(BookingStatus status, DateTime createdAtUtc, DateTime? paymentDeadlineUtc, DateTime nowUtc) =>
        status switch
        {
            BookingStatus.Pending => createdAtUtc.AddHours(BookingRules.PendingExpiryHours) <= nowUtc,
            BookingStatus.Accepted => paymentDeadlineUtc is DateTime deadline && deadline <= nowUtc,
            _ => false,
        };
}
