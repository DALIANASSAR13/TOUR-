using TourGuide.DAL.Enums;

namespace TourGuide.DAL.Entities;

// A tourist's booking of a guide. Status changes only through the BLL state machine (NFR-05).
public class Booking
{
    public int Id { get; set; }
    public string TouristId { get; set; } = string.Empty;
    public int GuideProfileId { get; set; }

    public BookingType BookingType { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public int? Hours { get; set; }
    public int? Days { get; set; }
    public int BillableHours { get; set; }
    public decimal PricePerHourAtRequest { get; set; }
    public decimal TotalPrice { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RespondedAtUtc { get; set; }
    public DateTime? PaymentDeadlineUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public CancellationReason? CancellationReason { get; set; }
    public DateTime? CompletedAtUtc { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ApplicationUser Tourist { get; set; } = null!;
    public GuideProfile GuideProfile { get; set; } = null!;
    public Payment? Payment { get; set; }
    public Review? Review { get; set; }
}
