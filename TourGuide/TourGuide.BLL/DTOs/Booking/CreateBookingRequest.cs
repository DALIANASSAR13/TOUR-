using TourGuide.DAL.Enums;

namespace TourGuide.BLL.DTOs.Booking;

// FR-11 / FR-12 input. Start date and time are in the guide's local time zone.
// 2.6.a (preview) uses type + hours/days; 2.6.b (create) also uses the start.
public class CreateBookingRequest
{
    public int GuideProfileId { get; set; }
    public BookingType BookingType { get; set; }
    public DateOnly? StartDate { get; set; }
    public TimeOnly? StartTime { get; set; }   // Hourly only
    public int? Hours { get; set; }            // Hourly only
    public int? Days { get; set; }             // MultiDay only
}
