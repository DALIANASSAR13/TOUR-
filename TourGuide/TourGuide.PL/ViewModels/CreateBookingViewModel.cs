using System.ComponentModel.DataAnnotations;
using TourGuide.BLL.Domain;
using TourGuide.DAL.Enums;

namespace TourGuide.PL.ViewModels;

// Booking request form (FR-11, FR-12). Limits come from BookingRules only.
// Hours/Days are required per type by CreateBookingValidator (BLL); here only the range is checked.
public class CreateBookingViewModel
{
    [Required]
    public int GuideProfileId { get; set; }

    [Required(ErrorMessage = "Choose hourly or multi-day.")]
    [Display(Name = "Trip type")]
    public BookingType BookingType { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Start date")]
    public DateOnly? StartDate { get; set; }

    [DataType(DataType.Time)]
    [Display(Name = "Start time")]
    public TimeOnly? StartTime { get; set; }

    [Range(BookingRules.MinHours, BookingRules.MaxHours, ErrorMessage = "Choose {1} to {2} hours.")]
    public int? Hours { get; set; }

    [Range(BookingRules.MinDays, BookingRules.MaxDays, ErrorMessage = "Choose {1} to {2} days.")]
    public int? Days { get; set; }

    // Display only, filled from the first quote on GET.
    public string GuideName { get; set; } = string.Empty;
    public PriceQuoteViewModel? Quote { get; set; }
}
