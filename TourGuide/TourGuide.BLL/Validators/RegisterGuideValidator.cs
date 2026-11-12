using FluentValidation;
using TourGuide.BLL.DTOs.Account;
using TourGuide.BLL.Infrastructure;

namespace TourGuide.BLL.Validators;

public class RegisterGuideValidator : AbstractValidator<RegisterGuideRequest>
{
    public RegisterGuideValidator(ICountryProvider countryProvider, IValidator<FileUpload> fileValidator)
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .Length(2, 100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(x => x.CountryCode)
            .NotEmpty()
            .Must(countryProvider.Exists)
            .WithMessage("Invalid country.");

        RuleFor(x => x.ProofOfResidence).NotNull().WithMessage("Upload your proof of residence.")
            .SetValidator(fileValidator);
        RuleFor(x => x.GovernmentId).NotNull().WithMessage("Upload your government ID.")
            .SetValidator(fileValidator);
        RuleFor(x => x.CriminalRecord).NotNull().WithMessage("Upload your criminal record certificate.")
            .SetValidator(fileValidator);
    }
}
