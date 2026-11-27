using TourGuide.BLL.DTOs.Review;
using TourGuide.DAL.Common;

namespace TourGuide.BLL.DTOs.Guide;

// FR-09 public guide profile. Public fields only: no email, phone, documents or verification data (NFR-02).
public class PublicGuideProfileDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryName { get; set; }      // set by GuideProfileService from ICountryProvider
    public string? PhotoFileName { get; set; }
    public string? Bio { get; set; }
    public int? YearsOfExperience { get; set; }
    public decimal PricePerHour { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public List<string> Languages { get; set; } = new();
    public List<string> Specializations { get; set; } = new();

    // One page of reviews, newest first (set by GuideProfileService).
    public PagedList<ReviewDto> Reviews { get; set; } = new(new List<ReviewDto>(), 1, 0, 0);
}
