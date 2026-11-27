using FluentValidation;
using TourGuide.BLL.Domain;
using TourGuide.BLL.DTOs.Search;
using TourGuide.BLL.Infrastructure;

namespace TourGuide.BLL.Validators;

// FR-10: min ≤ max price, prices 0 – max price per hour, page ≥ 1, country from the ISO list.
public class GuideSearchFilterValidator : AbstractValidator<GuideSearchFilter>
{
    private static readonly decimal MaxPrice = (decimal)BookingRules.MaxPricePerHour;

    public GuideSearchFilterValidator(ICountryProvider countries)
    {
        RuleFor(f => f.MinPrice)
            .InclusiveBetween(0m, MaxPrice).When(f => f.MinPrice.HasValue)
            .WithMessage($"Enter a price from 0 to {MaxPrice:0}.");

        RuleFor(f => f.MaxPrice)
            .InclusiveBetween(0m, MaxPrice).When(f => f.MaxPrice.HasValue)
            .WithMessage($"Enter a price from 0 to {MaxPrice:0}.");

        RuleFor(f => f.MinPrice)
            .LessThanOrEqualTo(f => f.MaxPrice!.Value)
            .When(f => f.MinPrice.HasValue && f.MaxPrice.HasValue)
            .WithMessage("Min price must be at most max price.");

        RuleFor(f => f.CountryCode)
            .Must(code => countries.Exists(code!))
            .When(f => !string.IsNullOrWhiteSpace(f.CountryCode))
            .WithMessage("Choose a country from the list.");

        RuleFor(f => f.LanguageId)
            .GreaterThan(0).When(f => f.LanguageId.HasValue)
            .WithMessage("Choose a language from the list.");

        RuleFor(f => f.SpecializationId)
            .GreaterThan(0).When(f => f.SpecializationId.HasValue)
            .WithMessage("Choose a specialization from the list.");

        RuleFor(f => f.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Choose page 1 or later.");
    }
}
