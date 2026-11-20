namespace TourGuide.BLL.DTOs.GuideVerification;

public class PendingGuideDto
{
    public int GuideProfileId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? CountryCode { get; set; }
    public int DocumentCount { get; set; }
    public DateTime SubmittedAtUtc { get; set; }
}