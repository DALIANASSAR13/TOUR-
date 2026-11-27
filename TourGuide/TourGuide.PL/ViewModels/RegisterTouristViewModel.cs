using System.ComponentModel.DataAnnotations;

namespace TourGuide.PL.ViewModels;

public class RegisterTouristViewModel
{
    [Required]
    [StringLength(100)]
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
}