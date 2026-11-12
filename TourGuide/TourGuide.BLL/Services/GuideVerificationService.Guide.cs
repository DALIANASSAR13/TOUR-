using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Options;
using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.GuideVerification;
using TourGuide.BLL.Options;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Enums;
using TourGuide.DAL.UnitOfWork;

namespace TourGuide.BLL.Services;

public partial class GuideVerificationService : IGuideVerificationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<ReapplyRequest> _reapplyValidator;
    private readonly FileStorageOptions _storageOptions;
    private readonly TimeProvider _time;

    public GuideVerificationService(
        IUnitOfWork uow,
        IMapper mapper,
        IValidator<ReapplyRequest> reapplyValidator,
        IOptions<FileStorageOptions> storageOptions,
        TimeProvider time)
    {
        _uow = uow;
        _mapper = mapper;
        _reapplyValidator = reapplyValidator;
        _storageOptions = storageOptions.Value;
        _time = time;
    }

    public async Task<Result<GuideStatusDto>> GetMyStatusAsync(string userId, CancellationToken ct = default)
    {
        var profile = await _uow.GuideProfiles.GetByUserIdAsync(userId, ct);
        if (profile is null)
            return Result<GuideStatusDto>.Fail(ErrorCodes.NotFound, "Guide profile was not found.");

        var dto = _mapper.Map<GuideStatusDto>(profile);
        return Result<GuideStatusDto>.Ok(dto);
    }

    public async Task<Result> ReapplyAsync(string userId, ReapplyRequest request, CancellationToken ct = default)
    {
        var validation = await _reapplyValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Invalid(validation.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToList());

        var profile = await _uow.GuideProfiles.GetByUserIdForUpdateAsync(userId, ct);
        if (profile is null)
            return Result.Fail(ErrorCodes.NotFound, "Guide profile was not found.");

        if (profile.VerificationStatus != VerificationStatus.Rejected)
            return Result.Fail(ErrorCodes.InvalidStatusChange,
                "Only rejected applications can submit new verification documents.");

        var nowUtc = _time.GetUtcNow().UtcDateTime;
        var uploads = new (DocumentType Type, DocumentUploadItem Item)[]
        {
            (DocumentType.ProofOfResidence, request.ProofOfResidence!),
            (DocumentType.GovernmentId, request.GovernmentId!),
            (DocumentType.CriminalRecord, request.CriminalRecord!)
        };

        var newlySavedFiles = new List<string>();
        var oldFilesToDelete = new List<string>();

        try
        {
            foreach (var (docType, item) in uploads)
            {
                var (storedName, normalizedContentType) = await SaveDocumentAsync(item, ct);
                newlySavedFiles.Add(storedName);

                var existingDoc = await _uow.GuideDocuments.GetByTypeAsync(profile.Id, docType, ct);
                if (existingDoc is not null)
                {
                    if (!string.IsNullOrWhiteSpace(existingDoc.StoredFileName))
                        oldFilesToDelete.Add(existingDoc.StoredFileName);

                    existingDoc.StoredFileName = storedName;
                    existingDoc.OriginalFileName = Path.GetFileName(item.OriginalFileName);
                    existingDoc.ContentType = normalizedContentType;
                    existingDoc.SizeBytes = item.Length;
                    existingDoc.UploadedAtUtc = nowUtc;
                    _uow.GuideDocuments.Update(existingDoc);
                }
                else
                {
                    await _uow.GuideDocuments.AddAsync(new GuideDocument
                    {
                        GuideProfileId = profile.Id,
                        DocumentType = docType,
                        StoredFileName = storedName,
                        OriginalFileName = Path.GetFileName(item.OriginalFileName),
                        ContentType = normalizedContentType,
                        SizeBytes = item.Length,
                        UploadedAtUtc = nowUtc
                    }, ct);
                }
            }

            profile.VerificationStatus = VerificationStatus.Pending;
            profile.SubmittedAtUtc = nowUtc;
            profile.RejectionReason = null;
            profile.ReviewedAtUtc = null;
            profile.ReviewedByAdminId = null;

            await _uow.SaveChangesAsync(ct);
        }
        catch
        {
            foreach (var f in newlySavedFiles)
                DeleteDocumentSilently(f);
            throw;
        }

        foreach (var oldFile in oldFilesToDelete)
            DeleteDocumentSilently(oldFile);

        return Result.Ok();
    }

    private async Task<(string StoredFileName, string ContentType)> SaveDocumentAsync(DocumentUploadItem item, CancellationToken ct)
    {
        if (item.Stream.CanSeek)
            item.Stream.Position = 0;

        var header = new byte[8];
        var read = await item.Stream.ReadAsync(header.AsMemory(0, header.Length), ct);
        if (item.Stream.CanSeek)
            item.Stream.Position = 0;

        string ext;
        string contentType;
        if (read >= 4 && header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46)
        {
            ext = ".pdf";
            contentType = "application/pdf";
        }
        else if (read >= 8 && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47)
        {
            ext = ".png";
            contentType = "image/png";
        }
        else
        {
            ext = ".jpg";
            contentType = "image/jpeg";
        }

        var storedFileName = $"{Guid.NewGuid():N}{ext}";
        var folder = Path.Combine(_storageOptions.RootPath, "uploads");
        Directory.CreateDirectory(folder);

        var fullPath = Path.Combine(folder, storedFileName);
        await using var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await item.Stream.CopyToAsync(fs, ct);

        return (storedFileName, contentType);
    }

    private void DeleteDocumentSilently(string storedFileName)
    {
        try
        {
            var fullPath = Path.Combine(_storageOptions.RootPath, "uploads", Path.GetFileName(storedFileName));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}