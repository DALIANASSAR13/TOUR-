namespace TourGuide.BLL.DTOs.GuideProfile;

public class GuideProfileEditDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? CountryCode { get; set; }
    public string? City { get; set; }
    public string? TimeZoneId { get; set; }
    public string? Bio { get; set; }
    public string? PhotoFileName { get; set; }
    public int? YearsOfExperience { get; set; }
    public decimal? PricePerHour { get; set; }
    public bool IsBookable { get; set; }

    public List<int> SelectedLanguageIds { get; set; } = new();
    public List<int> SelectedSpecializationIds { get; set; } = new();

    public List<LookupItemDto> AvailableLanguages { get; set; } = new();
    public List<LookupItemDto> AvailableSpecializations { get; set; } = new();
}