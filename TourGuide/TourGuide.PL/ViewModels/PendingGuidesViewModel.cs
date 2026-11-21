namespace TourGuide.PL.ViewModels;

public class PendingGuidesViewModel
{
    public IReadOnlyList<PendingGuideItemViewModel> Guides { get; set; } = Array.Empty<PendingGuideItemViewModel>();
}

public class PendingGuideItemViewModel
{
    public int GuideProfileId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? CountryCode { get; set; }
    public int DocumentCount { get; set; }
    public DateTime SubmittedAtUtc { get; set; }
}