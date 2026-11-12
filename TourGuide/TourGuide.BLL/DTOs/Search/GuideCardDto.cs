namespace TourGuide.BLL.DTOs.Search;

// One guide in the search results (FR-10). Price and rating are the stored values.
public class GuideCardDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryName { get; set; }      // set by SearchService from ICountryProvider
    public string? PhotoFileName { get; set; }
    public int? YearsOfExperience { get; set; }
    public decimal PricePerHour { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public List<string> Languages { get; set; } = new();
    public List<string> Specializations { get; set; } = new();
}
