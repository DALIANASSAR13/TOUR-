namespace TourGuide.BLL.Services.Interfaces;

// Sub-folders under FileStorage:RootPath. Use these names, never a string literal.
public static class FileFolders
{
    // The 3 verification documents of a guide (FR-02, FR-05, FR-07). Only Admins may read them (BR-03).
    public const string GuideDocuments = "guide-documents";
}
