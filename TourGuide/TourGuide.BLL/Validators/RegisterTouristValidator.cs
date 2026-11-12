using FluentValidation;
using TourGuide.BLL.DTOs.Account;
using TourGuide.BLL.Services.Interfaces;

namespace TourGuide.BLL.Validators;

public class RegisterTouristValidator
    : AbstractValidator<RegisterTouristRequest>
{
    public RegisterTouristValidator(ICountryProvider countryProvider)
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .Must(countryProvider.IsValidCountry)
            .WithMessage("Please select a valid country.");
    }
}