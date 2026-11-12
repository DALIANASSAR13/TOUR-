using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Options;
using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.GuideProfile;
using TourGuide.BLL.Options;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Exceptions;
using TourGuide.DAL.UnitOfWork;

namespace TourGuide.BLL.Services;

public class GuideProfileService : IGuideProfileService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateGuideProfileRequest> _validator;
    private readonly FileStorageOptions _storageOptions;

    public GuideProfileService(
        IUnitOfWork uow,
        IMapper mapper,
        IValidator<UpdateGuideProfileRequest> validator,
        IOptions<FileStorageOptions> storageOptions)
    {
        _uow = uow;
        _mapper = mapper;
        _validator = validator;
        _storageOptions = storageOptions.Value;
    }

    public async Task<Result<GuideProfileEditDto>> GetForEditAsync(string userId, CancellationToken ct = default)
    {
        var profile = await _uow.GuideProfiles.GetByUserIdForUpdateAsync(userId, ct);
        if (profile is null)
            return Result<GuideProfileEditDto>.Fail(ErrorCodes.NotFound, "Guide profile not found.");

        var dto = _mapper.Map<GuideProfileEditDto>(profile);
        await PopulateLookupsAsync(dto, ct);

        return Result<GuideProfileEditDto>.Ok(dto);
    }

    public async Task<Result<GuideProfileEditDto>> UpdateAsync(string userId, UpdateGuideProfileRequest request, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result<GuideProfileEditDto>.Invalid(validation.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToList());

        var profile = await _uow.GuideProfiles.GetByUserIdForUpdateAsync(userId, ct);
        if (profile is null)
            return Result<GuideProfileEditDto>.Fail(ErrorCodes.NotFound, "Guide profile not found.");

        // Validate selected LanguageIds & SpecializationIds exist in DB
        var allLanguages = await _uow.Languages.FindAsync(_ => true, ct);
        var validLangIds = allLanguages.Select(l => l.Id).ToHashSet();
        var distinctLangIds = request.LanguageIds.Distinct().ToList();
        if (distinctLangIds.Any(id => !validLangIds.Contains(id)))
            return Result<GuideProfileEditDto>.Invalid(new List<ValidationError>
            {
                new(nameof(UpdateGuideProfileRequest.LanguageIds), "One or more selected languages are invalid.")
            });

        var allSpecs = await _uow.Specializations.FindAsync(_ => true, ct);
        var validSpecIds = allSpecs.Select(s => s.Id).ToHashSet();
        var distinctSpecIds = request.SpecializationIds.Distinct().ToList();
        if (distinctSpecIds.Any(id => !validSpecIds.Contains(id)))
            return Result<GuideProfileEditDto>.Invalid(new List<ValidationError>
            {
                new(nameof(UpdateGuideProfileRequest.SpecializationIds), "One or more selected specializations are invalid.")
            });

        // Update ApplicationUser fields
        profile.User.FullName = request.FullName.Trim();
        profile.User.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
        profile.User.CountryCode = request.CountryCode.Trim().ToUpperInvariant();

        // Update GuideProfile fields
        profile.CountryCode = request.CountryCode.Trim().ToUpperInvariant();
        profile.City = request.City.Trim();
        profile.TimeZoneId = request.TimeZoneId.Trim();
        profile.Bio = string.IsNullOrWhiteSpace(request.Bio) ? null : request.Bio.Trim();
        profile.YearsOfExperience = request.YearsOfExperience;
        profile.PricePerHour = request.PricePerHour;

        // Sync Languages (remove deselected, add newly selected)
        var toRemoveLangs = profile.Languages.Where(l => !distinctLangIds.Contains(l.LanguageId)).ToList();
        foreach (var item in toRemoveLangs)
            profile.Languages.Remove(item);

        var existingLangIds = profile.Languages.Select(l => l.LanguageId).ToHashSet();
        foreach (var langId in distinctLangIds.Where(id => !existingLangIds.Contains(id)))
            profile.Languages.Add(new GuideLanguage { GuideProfileId = profile.Id, LanguageId = langId });

        // Sync Specializations
        var toRemoveSpecs = profile.Specializations.Where(s => !distinctSpecIds.Contains(s.SpecializationId)).ToList();
        foreach (var item in toRemoveSpecs)
            profile.Specializations.Remove(item);

        var existingSpecIds = profile.Specializations.Select(s => s.SpecializationId).ToHashSet();
        foreach (var specId in distinctSpecIds.Where(id => !existingSpecIds.Contains(id)))
            profile.Specializations.Add(new GuideSpecialization { GuideProfileId = profile.Id, SpecializationId = specId });

        // Handle optional profile photo upload
        string? newPhotoFileName = null;
        string? oldPhotoFileName = profile.PhotoFileName;

        if (request.PhotoStream is not null)
        {
            newPhotoFileName = await SavePhotoAsync(request.PhotoStream, request.PhotoContentType!, ct);
            profile.PhotoFileName = newPhotoFileName;
        }

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (ConcurrencyException)
        {
            if (newPhotoFileName is not null)
                DeletePhotoSilently(newPhotoFileName);

            return Result<GuideProfileEditDto>.Fail(ErrorCodes.Concurrency,
                "Your profile was updated in another session. Please refresh and try again.");
        }
        catch
        {
            if (newPhotoFileName is not null)
                DeletePhotoSilently(newPhotoFileName);
            throw;
        }

        // Delete old photo after successful DB commit
        if (newPhotoFileName is not null && !string.IsNullOrWhiteSpace(oldPhotoFileName))
            DeletePhotoSilently(oldPhotoFileName);

        var resultDto = _mapper.Map<GuideProfileEditDto>(profile);
        resultDto.AvailableLanguages = _mapper.Map<List<LookupItemDto>>(allLanguages.OrderBy(l => l.Name));
        resultDto.AvailableSpecializations = _mapper.Map<List<LookupItemDto>>(allSpecs.OrderBy(s => s.Name));

        return Result<GuideProfileEditDto>.Ok(resultDto);
    }

    private async Task PopulateLookupsAsync(GuideProfileEditDto dto, CancellationToken ct)
    {
        var languages = await _uow.Languages.FindAsync(_ => true, ct);
        var specializations = await _uow.Specializations.FindAsync(_ => true, ct);

        dto.AvailableLanguages = _mapper.Map<List<LookupItemDto>>(languages.OrderBy(l => l.Name));
        dto.AvailableSpecializations = _mapper.Map<List<LookupItemDto>>(specializations.OrderBy(s => s.Name));
    }

    private async Task<string> SavePhotoAsync(Stream stream, string contentType, CancellationToken ct)
    {
        var ext = contentType.Equals("image/png", StringComparison.OrdinalIgnoreCase) ? ".png" : ".jpg";
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var folder = Path.Combine(_storageOptions.RootPath, "guide-photos");
        Directory.CreateDirectory(folder);

        var fullPath = Path.Combine(folder, fileName);
        if (stream.CanSeek)
            stream.Position = 0;

        await using var fileStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(fileStream, ct);

        return fileName;
    }

    private void DeletePhotoSilently(string fileName)
    {
        try
        {
            var fullPath = Path.Combine(_storageOptions.RootPath, "guide-photos", Path.GetFileName(fileName));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}
