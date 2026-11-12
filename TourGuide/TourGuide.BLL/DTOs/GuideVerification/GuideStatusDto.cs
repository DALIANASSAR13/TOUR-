using TourGuide.DAL.Enums;

namespace TourGuide.BLL.DTOs.GuideVerification;

public class GuideStatusDto
{
    public VerificationStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime SubmittedAtUtc { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public string? TimeZoneId { get; set; }
    public bool IsProfileComplete { get; set; }
}