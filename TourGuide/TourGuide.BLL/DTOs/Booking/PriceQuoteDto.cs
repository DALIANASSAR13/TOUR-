using TourGuide.DAL.Enums;

namespace TourGuide.BLL.DTOs.Booking;

// FR-12: the price the tourist sees before sending the request. Nothing is saved.
public class PriceQuoteDto
{
    public int GuideProfileId { get; set; }
    public string GuideName { get; set; } = string.Empty;
    public BookingType BookingType { get; set; }
    public int? Hours { get; set; }
    public int? Days { get; set; }
    public int BillableHours { get; set; }
    public decimal PricePerHour { get; set; }
    public decimal TotalPrice { get; set; }
}
