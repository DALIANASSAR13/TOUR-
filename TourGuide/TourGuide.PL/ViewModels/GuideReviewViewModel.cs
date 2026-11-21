using TourGuide.DAL.Enums;

namespace TourGuide.PL.ViewModels;

public class GuideReviewViewModel
{
    public int GuideProfileId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }
    public string? CountryCode { get; set; }
    public string? TimeZoneId { get; set; }
    public decimal PricePerHour { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? SubmittedAtUtc { get; set; }
    public List<string> Languages { get; set; } = [];
    public List<string> Specializations { get; set; } = [];
    public List<GuideDocumentReviewItemViewModel> Documents { get; set; } = [];
}

public class GuideDocumentReviewItemViewModel
{
    public int Id { get; set; }
    public DocumentType DocumentType { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime UploadedAtUtc { get; set; }
}