using System.Globalization;
using TourGuide.BLL.Domain;

namespace TourGuide.PL.ViewModels;

// One review on the public guide profile. Shows the tourist's first name and country only (Q3).
public class ReviewItemViewModel
{
    private static readonly CultureInfo En = CultureInfo.InvariantCulture;

    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string TouristName { get; set; } = string.Empty;
    public string? TouristCountryCode { get; set; }
    public string? TouristCountryName { get; set; }

    public string FirstName =>
        TouristName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "Traveller";

    // "Emma · Germany"
    public string ByLine =>
        string.IsNullOrWhiteSpace(TouristCountryName) ? FirstName : $"{FirstName} · {TouristCountryName}";

    public string Initial => FirstName[..1];

    public string Stars => new string('★', Rating) + new string('☆', BookingRules.MaxRating - Rating);

    // "Oct 2026"
    public string DateText => CreatedAtUtc.ToString("MMM yyyy", En);
}
