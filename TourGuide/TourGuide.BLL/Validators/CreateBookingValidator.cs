using FluentValidation;
using TourGuide.BLL.Domain;
using TourGuide.BLL.DTOs.Booking;
using TourGuide.DAL.Enums;

namespace TourGuide.BLL.Validators;

// FR-11 / FR-12, BR-04 limits. 2.6.b adds: start >= now + MinAdvanceHours, same local day (TimeProvider).
public class CreateBookingValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingValidator()
    {
        RuleFor(x => x.GuideProfileId).GreaterThan(0).WithMessage("Choose a guide.");
        RuleFor(x => x.BookingType).IsInEnum().WithMessage("Choose hourly or multi-day.");

        When(x => x.BookingType == BookingType.Hourly, () =>
        {
            RuleFor(x => x.Hours)
                .NotNull().WithMessage($"Choose {BookingRules.MinHours} to {BookingRules.MaxHours} hours.")
                .InclusiveBetween(BookingRules.MinHours, BookingRules.MaxHours)
                .WithMessage($"Choose {BookingRules.MinHours} to {BookingRules.MaxHours} hours.");
            RuleFor(x => x.Days).Null().WithMessage("Leave days empty for an hourly trip.");
        });

        When(x => x.BookingType == BookingType.MultiDay, () =>
        {
            RuleFor(x => x.Days)
                .NotNull().WithMessage($"Choose {BookingRules.MinDays} to {BookingRules.MaxDays} days.")
                .InclusiveBetween(BookingRules.MinDays, BookingRules.MaxDays)
                .WithMessage($"Choose {BookingRules.MinDays} to {BookingRules.MaxDays} days.");
            RuleFor(x => x.Hours).Null().WithMessage("Leave hours empty for a multi-day trip.");
        });
    }
}
