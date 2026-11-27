namespace TourGuide.BLL.DTOs.Account;

public class RegisterGuideRequest
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string CountryCode { get; set; } = string.Empty;

    public FileUpload ProofOfResidence { get; set; } = null!;

    public FileUpload GovernmentId { get; set; } = null!;

    public FileUpload CriminalRecord { get; set; } = null!;
}
