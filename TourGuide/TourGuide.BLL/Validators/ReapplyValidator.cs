using FluentValidation;
using Microsoft.Extensions.Options;
using TourGuide.BLL.DTOs.GuideVerification;
using TourGuide.BLL.Options;

namespace TourGuide.BLL.Validators;

public class ReapplyValidator : AbstractValidator<ReapplyRequest>
{
    public ReapplyValidator(IOptions<FileStorageOptions> options)
    {
        var maxBytes = options.Value.MaxFileSizeBytes > 0
            ? options.Value.MaxFileSizeBytes
            : 5 * 1024 * 1024; // 5 MB

        RuleFor(x => x.ProofOfResidence)
            .NotNull().WithMessage("Proof of residence is required.")
            .Must(f => f is null || f.Length > 0).WithMessage("Proof of residence file cannot be empty.")
            .Must(f => f is null || f.Length <= maxBytes).WithMessage("Proof of residence must be 5 MB or less.")
            .Must(f => f is null || HasValidSignature(f)).WithMessage("Proof of residence must be a valid PDF, JPG, or PNG file.");

        RuleFor(x => x.GovernmentId)
            .NotNull().WithMessage("Government ID is required.")
            .Must(f => f is null || f.Length > 0).WithMessage("Government ID file cannot be empty.")
            .Must(f => f is null || f.Length <= maxBytes).WithMessage("Government ID must be 5 MB or less.")
            .Must(f => f is null || HasValidSignature(f)).WithMessage("Government ID must be a valid PDF, JPG, or PNG file.");

        RuleFor(x => x.CriminalRecord)
            .NotNull().WithMessage("Criminal record certificate is required.")
            .Must(f => f is null || f.Length > 0).WithMessage("Criminal record certificate file cannot be empty.")
            .Must(f => f is null || f.Length <= maxBytes).WithMessage("Criminal record certificate must be 5 MB or less.")
            .Must(f => f is null || HasValidSignature(f)).WithMessage("Criminal record certificate must be a valid PDF, JPG, or PNG file.");
    }

    private static bool HasValidSignature(DocumentUploadItem file)
    {
        if (file.Stream is null || !file.Stream.CanRead)
            return false;

        var header = new byte[8];
        if (file.Stream.CanSeek)
            file.Stream.Position = 0;

        var read = file.Stream.Read(header, 0, header.Length);
        if (file.Stream.CanSeek)
            file.Stream.Position = 0;

        if (read < 4)
            return false;

        // PDF: %PDF
        bool isPdf = header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46;
        // JPEG: FF D8 FF
        bool isJpg = header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
        // PNG: 89 50 4E 47 0D 0A 1A 0A
        bool isPng = read >= 8 &&
                     header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 &&
                     header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A;

        return isPdf || isJpg || isPng;
    }
}