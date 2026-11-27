namespace TourGuide.BLL.DTOs.Review;

// FR-19 input: a star rating and an optional comment for one booking. Limits: BookingRules.
public class CreateReviewRequest
{
    public int BookingId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
