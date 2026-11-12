using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Claims;
using TourGuide.BLL.DTOs.GuideProfile;
using TourGuide.BLL.Options;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Constants;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Areas.Guide.Controllers;

[Area("Guide")]
[Authorize(Roles = Roles.Guide)]
public class ProfileController : Controller
{
    private readonly IGuideProfileService _profileService;
    private readonly FileStorageOptions _storageOptions;

    public ProfileController(
        IGuideProfileService profileService,
        IOptions<FileStorageOptions> storageOptions)
    {
        _profileService = profileService;
        _storageOptions = storageOptions.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Edit(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Challenge();

        var result = await _profileService.GetForEditAsync(userId, ct);
        if (!result.Succeeded || result.Value is null)
            return NotFound();

        var vm = MapToViewModel(result.Value);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditGuideProfileViewModel vm, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Challenge();

        await using var photoStream = vm.Photo?.OpenReadStream();

        var request = new UpdateGuideProfileRequest
        {
            FullName = vm.FullName,
            PhoneNumber = vm.PhoneNumber,
            CountryCode = vm.CountryCode,
            City = vm.City,
            TimeZoneId = vm.TimeZoneId,
            Bio = vm.Bio,
            YearsOfExperience = vm.YearsOfExperience,
            PricePerHour = vm.PricePerHour ?? 0,
            LanguageIds = vm.LanguageIds ?? new List<int>(),
            SpecializationIds = vm.SpecializationIds ?? new List<int>(),
            PhotoStream = photoStream,
            PhotoContentType = vm.Photo?.ContentType,
            PhotoLength = vm.Photo?.Length,
            PhotoOriginalFileName = vm.Photo?.FileName
        };

        var result = await _profileService.UpdateAsync(userId, request, ct);

        if (!result.Succeeded)
        {
            if (result.Errors.Any())
            {
                foreach (var err in result.Errors)
                {
                    var key = err.Field == nameof(UpdateGuideProfileRequest.PhotoLength) ||
                              err.Field == nameof(UpdateGuideProfileRequest.PhotoContentType)
                        ? nameof(EditGuideProfileViewModel.Photo)
                        : err.Field;

                    ModelState.AddModelError(key, err.Message);
                }
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                ModelState.AddModelError(string.Empty, result.Message);
            }

            // Reload lookups & current photo so the form re-renders cleanly
            var current = await _profileService.GetForEditAsync(userId, ct);
            if (current.Succeeded && current.Value is not null)
            {
                vm.CurrentPhotoFileName = current.Value.PhotoFileName;
                vm.IsBookable = current.Value.IsBookable;
                vm.AvailableLanguages = current.Value.AvailableLanguages;
                vm.AvailableSpecializations = current.Value.AvailableSpecializations;
            }
            PopulateDropdowns(vm);
            return View(vm);
        }

        TempData["ProfileSuccess"] = "Your profile has been updated successfully.";
        return RedirectToAction(nameof(Edit));
    }

    // Safely serves guide photos stored outside wwwroot (NFR-02)
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Photo(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return NotFound();

        var safeName = Path.GetFileName(fileName);
        var fullPath = Path.Combine(_storageOptions.RootPath, "guide-photos", safeName);
        if (!System.IO.File.Exists(fullPath))
            return NotFound();

        var contentType = safeName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
            ? "image/png"
            : "image/jpeg";

        return PhysicalFile(Path.GetFullPath(fullPath), contentType);
    }

    private EditGuideProfileViewModel MapToViewModel(GuideProfileEditDto dto)
    {
        var vm = new EditGuideProfileViewModel
        {
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            CountryCode = dto.CountryCode ?? "EG",
            City = dto.City ?? string.Empty,
            TimeZoneId = dto.TimeZoneId ?? string.Empty,
            Bio = dto.Bio,
            YearsOfExperience = dto.YearsOfExperience,
            PricePerHour = dto.PricePerHour,
            LanguageIds = dto.SelectedLanguageIds,
            SpecializationIds = dto.SelectedSpecializationIds,
            CurrentPhotoFileName = dto.PhotoFileName,
            IsBookable = dto.IsBookable,
            AvailableLanguages = dto.AvailableLanguages,
            AvailableSpecializations = dto.AvailableSpecializations
        };

        PopulateDropdowns(vm);
        return vm;
    }

    private static void PopulateDropdowns(EditGuideProfileViewModel vm)
    {
        vm.Countries = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(c =>
            {
                try { return new RegionInfo(c.Name); }
                catch { return null; }
            })
            .Where(r => r is not null && r.TwoLetterISORegionName.Length == 2)
            .DistinctBy(r => r!.TwoLetterISORegionName)
            .OrderBy(r => r!.EnglishName)
            .Select(r => new SelectListItem(r!.EnglishName, r.TwoLetterISORegionName))
            .ToList();

        vm.TimeZones = TimeZoneInfo.GetSystemTimeZones()
            .Select(tz => new SelectListItem(tz.DisplayName, tz.Id))
            .ToList();
    }
}