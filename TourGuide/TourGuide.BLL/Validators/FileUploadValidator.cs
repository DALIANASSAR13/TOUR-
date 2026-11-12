using FluentValidation;
using Microsoft.Extensions.Options;
using TourGuide.BLL.DTOs.Account;
using TourGuide.BLL.Options;
using TourGuide.BLL.Services.Account;

namespace TourGuide.BLL.Validators;

public class FileUploadValidator : AbstractValidator<FileUpload>
{
    public FileUploadValidator(IOptions<FileStorageOptions> options)
    {
        var max = options.Value.MaxFileSizeBytes;

        RuleFor(x => x.SizeBytes)
            .GreaterThan(0).WithMessage("The file is empty. Choose another file.")
            .LessThanOrEqualTo(max).WithMessage($"The file is larger than {max / 1024 / 1024} MB. Choose a smaller file.");

        RuleFor(x => x.Content)
            .Must(stream => FileSignatureDetector.Detect(stream) != null)
            .WithMessage("Upload a PDF, JPG or PNG file.");
    }
}