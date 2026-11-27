using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using TourGuide.PL.Validation;

namespace TourGuide.PL.ViewModels;

public class RegisterGuideViewModel
{
    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string CountryCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Upload your proof of residence.")]
    [MaxFileSize]
    [AllowedFileTypes]
    public IFormFile? ProofOfResidence { get; set; }

    [Required(ErrorMessage = "Upload your government ID.")]
    [MaxFileSize]
    [AllowedFileTypes]
    public IFormFile? GovernmentId { get; set; }

    [Required(ErrorMessage = "Upload your criminal record certificate.")]
    [MaxFileSize]
    [AllowedFileTypes]
    public IFormFile? CriminalRecord { get; set; }
}
