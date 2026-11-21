using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.GuideVerification;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Constants;

namespace TourGuide.BLL.Services;

public partial class GuideVerificationService
{
    public async Task<Result<IReadOnlyList<PendingGuideDto>>> GetPendingAsync(CancellationToken ct = default)
    {
        var pendingGuides = await _uow.GuideProfiles.GetPendingAsync(ct);
        var dtos = _mapper.Map<IReadOnlyList<PendingGuideDto>>(pendingGuides);
        return Result<IReadOnlyList<PendingGuideDto>>.Ok(dtos);
    }

    public async Task<Result<GuideReviewDto>> GetForReviewAsync(int guideProfileId, CancellationToken ct = default)
    {
        var profile = await _uow.GuideProfiles.GetWithDetailsAsync(guideProfileId, ct);
        if (profile is null)
            return Result<GuideReviewDto>.Fail(ErrorCodes.NotFound, "Guide profile was not found.");

        var documents = await _uow.GuideDocuments.GetByGuideAsync(guideProfileId, ct);

        var dto = new GuideReviewDto
        {
            GuideProfileId = profile.Id,
            UserId = profile.UserId,
            FullName = profile.User?.FullName ?? string.Empty,
            Email = profile.User?.Email ?? string.Empty,
            PhoneNumber = profile.User?.PhoneNumber,
            Bio = profile.Bio,
            City = profile.City,
            CountryCode = profile.CountryCode,
            TimeZoneId = profile.TimeZoneId,
            PricePerHour = profile.PricePerHour.GetValueOrDefault(),
            VerificationStatus = profile.VerificationStatus,
            RejectionReason = profile.RejectionReason,
            SubmittedAtUtc = profile.SubmittedAtUtc,
            Languages = profile.Languages
                .Where(l => l.Language != null)
                .Select(l => l.Language.Name)
                .OrderBy(n => n)
                .ToList(),
            Specializations = profile.Specializations
                .Where(s => s.Specialization != null)
                .Select(s => s.Specialization.Name)
                .OrderBy(n => n)
                .ToList(),
            Documents = documents.Select(d => new GuideDocumentItemDto
            {
                Id = d.Id,
                DocumentType = d.DocumentType,
                OriginalFileName = d.OriginalFileName,
                ContentType = d.ContentType,
                SizeBytes = d.SizeBytes,
                UploadedAtUtc = d.UploadedAtUtc
            }).ToList()
        };

        return Result<GuideReviewDto>.Ok(dto);
    }

    public async Task<Result<DocumentStreamDto>> OpenDocumentAsync(int documentId, CancellationToken ct = default)
    {
        var doc = await _uow.GuideDocuments.GetByIdAsync(documentId, ct);
        if (doc is null)
            return Result<DocumentStreamDto>.Fail(ErrorCodes.NotFound, "Document was not found.");

        var safeFileName = Path.GetFileName(doc.StoredFileName);
        var candidatePaths = new[]
        {
            // Current storage layout: FileStorage:RootPath/guide-documents.
            Path.Combine(_storageOptions.RootPath, FileFolders.GuideDocuments, safeFileName),
            // Legacy layouts kept for documents uploaded before the storage fix.
            Path.Combine(_storageOptions.RootPath, safeFileName),
            Path.Combine(_storageOptions.RootPath, "uploads", safeFileName)
        };

        var fullPath = candidatePaths.FirstOrDefault(File.Exists);
        if (fullPath is null)
            return Result<DocumentStreamDto>.Fail(ErrorCodes.NotFound, "Document file was not found on disk.");

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Result<DocumentStreamDto>.Ok(new DocumentStreamDto
        {
            Stream = stream,
            ContentType = string.IsNullOrWhiteSpace(doc.ContentType) ? "application/octet-stream" : doc.ContentType,
            OriginalFileName = doc.OriginalFileName
        });
    }
}