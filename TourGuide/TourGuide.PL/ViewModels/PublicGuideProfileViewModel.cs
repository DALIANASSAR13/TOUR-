using TourGuide.BLL.Domain;

namespace TourGuide.PL.ViewModels;

// FR-09 public guide profile. Reuses the search card's fields and display helpers (name, location, price, rating).
public class PublicGuideProfileViewModel : GuideCardViewModel
{
    public string? Bio { get; set; }

    public List<ReviewItemViewModel> Reviews { get; set; } = new();
    public int ReviewPage { get; set; }
    public int ReviewTotalPages { get; set; }

    // Book is shown to visitors and tourists; guides and admins cannot book (NFR-01). Set by the controller.
    public bool CanBook { get; set; }

    public string FirstName =>
        FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? FullName;

    public string Stars
    {
        get
        {
            var full = (int)Math.Round(AverageRating, MidpointRounding.AwayFromZero);
            return new string('★', full) + new string('☆', BookingRules.MaxRating - full);
        }
    }
}
