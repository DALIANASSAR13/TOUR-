namespace TourGuide.BLL.Options;

// Bound from appsettings.json "FileStorage". RootPath must be outside wwwroot (NFR-02).
public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    public string RootPath { get; set; } = "App_Data/uploads";
    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;
    public List<string> AllowedContentTypes { get; set; } = new() { "application/pdf", "image/jpeg", "image/png" };
}
