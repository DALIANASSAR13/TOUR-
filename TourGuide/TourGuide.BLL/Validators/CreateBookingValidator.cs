using FluentValidation;
using TourGuide.BLL.Domain;
using TourGuide.BLL.DTOs.Booking;
using TourGuide.DAL.Enums;
using TourGuide.DAL.UnitOfWork;

namespace TourGuide.BLL.Validators;

// FR-11 / FR-12, BR-04.
// Default rules (limits) run for the price preview and for create.
// The "Create" rule set adds the start: date/time required, same local day, start >= now + MinAdvanceHours.
public class CreateBookingValidator : AbstractValidator<CreateBookingRequest>
{
    public const string CreateRuleSet = "Create";

    private static readonly string HoursMessage = $"Choose {BookingRules.MinHours} to {BookingRules.MaxHours} hours.";
    private static readonly string DaysMessage = $"Choose {BookingRules.MinDays} to {BookingRules.MaxDays} days.";

    public CreateBookingValidator(IUnitOfWork uow, TimeProvider time)
    {
        RuleFor(x => x.GuideProfileId).GreaterThan(0).WithMessage("Choose a guide.");
        RuleFor(x => x.BookingType).IsInEnum().WithMessage("Choose hourly or multi-day.");

        When(x => x.BookingType == BookingType.Hourly, () =>
        {
            RuleFor(x => x.Hours).NotNull().WithMessage(HoursMessage)
                .InclusiveBetween(BookingRules.MinHours, BookingRules.MaxHours).WithMessage(HoursMessage);
            RuleFor(x => x.Days).Null().WithMessage("Leave days empty for an hourly trip.");
        });

        When(x => x.BookingType == BookingType.MultiDay, () =>
        {
            RuleFor(x => x.Days).NotNull().WithMessage(DaysMessage)
                .InclusiveBetween(BookingRules.MinDays, BookingRules.MaxDays).WithMessage(DaysMessage);
            RuleFor(x => x.Hours).Null().WithMessage("Leave hours empty for a multi-day trip.");
        });

        RuleSet(CreateRuleSet, () =>
        {
            RuleFor(x => x.StartDate).NotNull().WithMessage("Choose a start date.");

            When(x => x.BookingType == BookingType.Hourly, () =>
            {
                RuleFor(x => x.StartTime).NotNull().WithMessage("Choose a start time.");

                // BR-04: an hourly trip ends on the day it starts (ending exactly at midnight is allowed).
                RuleFor(x => x.StartTime)
                    .Must((r, t) => t!.Value.ToTimeSpan() + TimeSpan.FromHours(r.Hours!.Value) <= TimeSpan.FromDays(1))
                    .When(r => r.StartTime is not null && r.Hours is >= BookingRules.MinHours and <= BookingRules.MaxHours)
                    .WithMessage("The trip must end on the same day. Start earlier or choose fewer hours.");
            });

            // BR-04: start >= now + MinAdvanceHours, in the guide's time zone (read through IUnitOfWork, no writes).
            RuleFor(x => x.StartDate)
                .CustomAsync(async (_, ctx, ct) =>
                {
                    var r = ctx.InstanceToValidate;
                    var guide = await uow.GuideProfiles.GetByIdAsync(r.GuideProfileId, ct);
                    if (guide?.TimeZoneId is null)
                        return; // unbookable guide: BookingService refuses it

                    var zone = BookingTimes.FindZone(guide.TimeZoneId);
                    if (r.BookingType == BookingType.Hourly && r.StartDate is { } d && r.StartTime is { } t
        && !BookingTimes.TryLocalToUtc(d.ToDateTime(t), zone, out DateTime _))
                    {
                        ctx.AddFailure(nameof(r.StartTime), "This time does not exist on that day (clock change). Choose another time.");
                        return;
                    }

                    var times = BookingTimes.Resolve(r, zone);
                    var earliest = time.GetUtcNow().UtcDateTime.AddHours(BookingRules.MinAdvanceHours);
                    if (times is { } span && span.StartUtc < earliest)
                        ctx.AddFailure(nameof(r.StartDate),
                            $"The trip must start at least {BookingRules.MinAdvanceHours} hours from now.");
                })
                .When(r => r.StartDate is not null);
        });
    }
}
