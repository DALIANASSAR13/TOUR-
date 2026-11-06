using TourGuide.DAL.Enums;

namespace TourGuide.DAL.Entities;

// Everything about a guide beyond the login account. Public fields are filled after approval (FR-08).
public class GuideProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;

    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public string? RejectionReason { get; set; }
    public DateTime SubmittedAtUtc { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public string? ReviewedByAdminId { get; set; }

    public string? CountryCode { get; set; }
    public string? City { get; set; }
    public string? TimeZoneId { get; set; }
    public string? Bio { get; set; }
    public string? PhotoFileName { get; set; }
    public int? YearsOfExperience { get; set; }
    public decimal? PricePerHour { get; set; }

    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public ApplicationUser? ReviewedByAdmin { get; set; }
    public ICollection<GuideDocument> Documents { get; set; } = new List<GuideDocument>();
    public ICollection<GuideLanguage> Languages { get; set; } = new List<GuideLanguage>();
    public ICollection<GuideSpecialization> Specializations { get; set; } = new List<GuideSpecialization>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
