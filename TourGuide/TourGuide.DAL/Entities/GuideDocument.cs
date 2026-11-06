using TourGuide.DAL.Enums;

namespace TourGuide.DAL.Entities;

// One uploaded verification file. One current file per document type.
public class GuideDocument
{
    public int Id { get; set; }
    public int GuideProfileId { get; set; }
    public DocumentType DocumentType { get; set; }
    public string StoredFileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime UploadedAtUtc { get; set; }

    public GuideProfile GuideProfile { get; set; } = null!;
}
