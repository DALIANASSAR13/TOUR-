using TourGuide.DAL.Enums;

namespace TourGuide.BLL.DTOs.Booking;

// One booking as the tourist (FR-15) or the guide (FR-16) sees it.
public class BookingDto
{
    public int Id { get; set; }

    // Guide
    public int GuideProfileId { get; set; }
    public string GuideName { get; set; } = string.Empty;
    public string? GuideCity { get; set; }
    public string? GuideCountryCode { get; set; }
    public string? GuidePhotoFileName { get; set; }
    public string? GuideTimeZoneId { get; set; }

    // Trip and price (copied from the booking, never recalculated)
    public BookingType BookingType { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public int? Hours { get; set; }
    public int? Days { get; set; }
    public int BillableHours { get; set; }
    public decimal PricePerHourAtRequest { get; set; }
    public decimal TotalPrice { get; set; }

    // Status
    public BookingStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RespondedAtUtc { get; set; }
    public DateTime? PaymentDeadlineUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public CancellationReason? CancellationReason { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public bool IsPaid { get; set; }
    public int? ReviewRating { get; set; }
    public string? ReviewComment { get; set; }

    // Set by BookingService from the rules (BR-06, BR-07, BR-13), not by the mapping profile.
    public DateTime? AutoCancelAtUtc { get; set; }
    public bool CanPay { get; set; }
    public bool CanWithdraw { get; set; }
    public bool CanReview { get; set; }
}
