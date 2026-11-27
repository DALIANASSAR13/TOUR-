namespace TourGuide.BLL.DTOs.GuideVerification;

public class DocumentUploadItem
{
    public Stream Stream { get; set; } = Stream.Null;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Length { get; set; }
}

public class ReapplyRequest
{
    public DocumentUploadItem? ProofOfResidence { get; set; }
    public DocumentUploadItem? GovernmentId { get; set; }
    public DocumentUploadItem? CriminalRecord { get; set; }
}