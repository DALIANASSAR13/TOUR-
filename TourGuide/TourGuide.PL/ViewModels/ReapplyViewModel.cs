using System.ComponentModel.DataAnnotations;

namespace TourGuide.PL.ViewModels;

public class ReapplyViewModel
{
    public string? PreviousRejectionReason { get; set; }

    [Required(ErrorMessage = "Please upload your proof of residence.")]
    [Display(Name = "Proof of Residence (PDF, JPG, or PNG, max 5 MB)")]
    public IFormFile? ProofOfResidence { get; set; }

    [Required(ErrorMessage = "Please upload your government ID.")]
    [Display(Name = "Government ID (PDF, JPG, or PNG, max 5 MB)")]
    public IFormFile? GovernmentId { get; set; }

    [Required(ErrorMessage = "Please upload your criminal record certificate.")]
    [Display(Name = "Criminal Record Certificate (PDF, JPG, or PNG, max 5 MB)")]
    public IFormFile? CriminalRecord { get; set; }
}