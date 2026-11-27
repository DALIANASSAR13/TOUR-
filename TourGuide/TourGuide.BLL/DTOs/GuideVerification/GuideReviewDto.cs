using TourGuide.DAL.Enums;

namespace TourGuide.BLL.DTOs.GuideVerification;

public class GuideReviewDto
{
    public int GuideProfileId { get; set; }
    public string UserId { get; set; } = string.Empty;
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
    public List<GuideDocumentItemDto> Documents { get; set; } = [];
}

public class GuideDocumentItemDto
{
    public int Id { get; set; }
    public DocumentType DocumentType { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime UploadedAtUtc { get; set; }
}

public class DocumentStreamDto
{
    public Stream Stream { get; set; } = Stream.Null;
    public string ContentType { get; set; } = "application/octet-stream";
    public string OriginalFileName { get; set; } = string.Empty;
}