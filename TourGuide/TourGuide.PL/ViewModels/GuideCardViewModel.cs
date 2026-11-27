using System.Globalization;

namespace TourGuide.PL.ViewModels;

// One guide card in the search results. Settable fields come from GuideCardDto by AutoMapper;
// the get-only properties only format them for display.
public class GuideCardViewModel
{
    private static readonly CultureInfo En = CultureInfo.InvariantCulture;

    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryName { get; set; }
    public string? PhotoFileName { get; set; }
    public int? YearsOfExperience { get; set; }
    public decimal PricePerHour { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public List<string> Languages { get; set; } = new();
    public List<string> Specializations { get; set; } = new();

    public string Initials =>
        string.Concat(FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(p => p[0]));

    public string Location =>
        string.Join(", ", new[] { City, CountryName ?? CountryCode }.Where(x => !string.IsNullOrWhiteSpace(x)));

    // "$25" / "$22.50"
    public string PriceText => "$" + PricePerHour.ToString(PricePerHour % 1 != 0 ? "#,0.00" : "#,0", En);

    public bool HasReviews => ReviewCount > 0;

    // "4.8" and "12 reviews"
    public string RatingText => AverageRating.ToString("0.0", En);
    public string ReviewCountText => ReviewCount == 1 ? "1 review" : $"{ReviewCount} reviews";

    public string? ExperienceText => YearsOfExperience switch
    {
        null => null,
        1 => "1 year experience",
        var y => $"{y} years experience",
    };
}
