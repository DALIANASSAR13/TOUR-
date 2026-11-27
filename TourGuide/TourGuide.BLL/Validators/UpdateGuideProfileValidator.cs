using FluentValidation;
using Microsoft.Extensions.Options;
using TourGuide.BLL.DTOs.GuideProfile;
using TourGuide.BLL.Options;

namespace TourGuide.BLL.Validators;

public class UpdateGuideProfileValidator : AbstractValidator<UpdateGuideProfileRequest>
{
    private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png"
    };

    public UpdateGuideProfileValidator(IOptions<FileStorageOptions> fileOptions)
    {
        var storage = fileOptions.Value;

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30).WithMessage("Phone number is too long.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.CountryCode)
            .NotEmpty().WithMessage("Country is required.")
            .Length(2).WithMessage("Select a valid 2-letter country code.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

        RuleFor(x => x.TimeZoneId)
            .NotEmpty().WithMessage("Time zone is required.")
            .Must(BeValidTimeZone).WithMessage("Select a valid time zone.");

        RuleFor(x => x.Bio)
            .MaximumLength(2000).WithMessage("Bio cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Bio));

        RuleFor(x => x.YearsOfExperience)
            .InclusiveBetween(0, 60).WithMessage("Years of experience must be between 0 and 60.")
            .When(x => x.YearsOfExperience.HasValue);

        RuleFor(x => x.PricePerHour)
            .GreaterThan(0).WithMessage("Hourly price must be greater than 0.")
            .LessThanOrEqualTo(10000).WithMessage("Hourly price seems too high.");

        RuleFor(x => x.LanguageIds)
            .NotEmpty().WithMessage("Select at least one language.");

        RuleFor(x => x.SpecializationIds)
            .NotEmpty().WithMessage("Select at least one specialization.");

        When(x => x.PhotoStream is not null, () =>
        {
            RuleFor(x => x.PhotoLength)
                .NotNull()
                .GreaterThan(0).WithMessage("Uploaded photo is empty.")
                .LessThanOrEqualTo(storage.MaxFileSizeBytes)
                .WithMessage($"Photo size cannot exceed {storage.MaxFileSizeBytes / (1024 * 1024)} MB.");

            RuleFor(x => x.PhotoContentType)
                .NotEmpty()
                .Must(ct => ct is not null && AllowedImageTypes.Contains(ct))
                .WithMessage("Only JPG and PNG images are allowed.");
        });
    }

    private static bool BeValidTimeZone(string? timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId)) return false;
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
    }
}