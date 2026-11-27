namespace TourGuide.BLL.DTOs.Review;

// One review as shown on the public guide profile (FR-09). Read-only.
public class ReviewDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string TouristName { get; set; } = string.Empty;
    public string? TouristCountryCode { get; set; }
    public string? TouristCountryName { get; set; }  // set by GuideProfileService from ICountryProvider
}
