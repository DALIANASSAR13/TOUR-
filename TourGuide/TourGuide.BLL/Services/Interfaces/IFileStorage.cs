using TourGuide.BLL.DTOs.Account;

namespace TourGuide.BLL.Services.Interfaces;

// Saves, reads, replaces and deletes files outside wwwroot (NFR-02).
// "folder" is one of FileFolders. The stored name is a new GUID plus an extension, never the user's file name.
public interface IFileStorage
{
    // Returns the stored file name.
    Task<string> SaveAsync(FileUpload file, string folder, CancellationToken ct = default);

    // Throws FileNotFoundException when the file does not exist.
    Task<Stream> OpenReadAsync(string storedName, string folder, CancellationToken ct = default);

    // Saves the new file, then removes the old one. Returns the new stored file name.
    Task<string> ReplaceAsync(string oldName, FileUpload file, string folder, CancellationToken ct = default);

    // Removes a file. Does nothing if it is already gone.
    Task DeleteAsync(string storedName, string folder, CancellationToken ct = default);
}
