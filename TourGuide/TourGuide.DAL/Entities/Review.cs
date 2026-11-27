namespace TourGuide.DAL.Entities;

// A tourist's rating of a guide for one completed booking.
public class Review
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string TouristId { get; set; } = string.Empty;
    public int GuideProfileId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public Booking Booking { get; set; } = null!;
    public ApplicationUser Tourist { get; set; } = null!;
    public GuideProfile GuideProfile { get; set; } = null!;
}
