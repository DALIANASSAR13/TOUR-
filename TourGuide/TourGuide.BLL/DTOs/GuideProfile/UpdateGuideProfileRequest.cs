namespace TourGuide.BLL.DTOs.GuideProfile;

public class UpdateGuideProfileRequest
{
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string TimeZoneId { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int? YearsOfExperience { get; set; }
    public decimal PricePerHour { get; set; }

    public List<int> LanguageIds { get; set; } = new();
    public List<int> SpecializationIds { get; set; } = new();

    // Optional uploaded photo stream (PL passes IFormFile properties cleanly without coupling BLL to ASP.NET Core)
    public Stream? PhotoStream { get; set; }
    public string? PhotoContentType { get; set; }
    public long? PhotoLength { get; set; }
    public string? PhotoOriginalFileName { get; set; }
}