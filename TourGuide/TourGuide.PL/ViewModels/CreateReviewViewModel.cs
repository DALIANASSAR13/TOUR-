using System.ComponentModel.DataAnnotations;
using TourGuide.BLL.Domain;

namespace TourGuide.PL.ViewModels;

// Review form (FR-19). Limits come from BookingRules only; CreateReviewValidator (BLL) checks them again.
public class CreateReviewViewModel
{
    public int BookingId { get; set; }

    [Required(ErrorMessage = "Choose a star rating.")]
    [Range(BookingRules.MinRating, BookingRules.MaxRating, ErrorMessage = "Choose {1} to {2} stars.")]
    public int? Rating { get; set; }

    [StringLength(BookingRules.MaxCommentLength, ErrorMessage = "Write at most {1} characters.")]
    [Display(Name = "Comment (optional)")]
    public string? Comment { get; set; }

    // Display only: the same "TG-0042" format as My Bookings.
    public string Reference => BookingItemViewModel.RefOf(BookingId);
}
