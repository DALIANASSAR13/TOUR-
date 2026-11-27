using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TourGuide.BLL.DTOs.GuideProfile;

namespace TourGuide.PL.ViewModels;

public class EditGuideProfileViewModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Country is required.")]
    [Display(Name = "Country")]
    public string CountryCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required.")]
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Time zone is required.")]
    [Display(Name = "Time Zone")]
    public string TimeZoneId { get; set; } = string.Empty;

    [Display(Name = "About Me (Bio)")]
    public string? Bio { get; set; }

    [Display(Name = "Years of Experience")]
    public int? YearsOfExperience { get; set; }

    [Required(ErrorMessage = "Hourly rate is required.")]
    [Display(Name = "Price Per Hour ($)")]
    public decimal? PricePerHour { get; set; }

    [Display(Name = "Languages Spoken")]
    public List<int> LanguageIds { get; set; } = new();

    [Display(Name = "Specializations")]
    public List<int> SpecializationIds { get; set; } = new();

    [Display(Name = "Profile Photo (JPG or PNG, max 5 MB)")]
    public IFormFile? Photo { get; set; }

    // Display-only properties
    public string? CurrentPhotoFileName { get; set; }
    public bool IsBookable { get; set; }

    public List<LookupItemDto> AvailableLanguages { get; set; } = new();
    public List<LookupItemDto> AvailableSpecializations { get; set; } = new();
    public List<SelectListItem> Countries { get; set; } = new();
    public List<SelectListItem> TimeZones { get; set; } = new();
}