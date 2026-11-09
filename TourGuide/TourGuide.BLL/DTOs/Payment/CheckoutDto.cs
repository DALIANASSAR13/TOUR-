using TourGuide.DAL.Enums;

namespace TourGuide.BLL.DTOs.Payment;

// FR-14: what the tourist is about to pay for. Copied from the booking, never recalculated.
// Holds no card data (NFR-04).
public class CheckoutDto
{
    // The booking's id.
    public int Id { get; set; }

    // Guide
    public int GuideProfileId { get; set; }
    public string GuideName { get; set; } = string.Empty;
    public string? GuideCity { get; set; }
    public string? GuideCountryCode { get; set; }
    public string? GuideTimeZoneId { get; set; }

    // Trip and price
    public BookingType BookingType { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public int? Hours { get; set; }
    public int? Days { get; set; }
    public int BillableHours { get; set; }
    public decimal PricePerHourAtRequest { get; set; }
    public decimal TotalPrice { get; set; }

    // Always Accepted and before the deadline when PaymentService returns it.
    public BookingStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? PaymentDeadlineUtc { get; set; }
}
