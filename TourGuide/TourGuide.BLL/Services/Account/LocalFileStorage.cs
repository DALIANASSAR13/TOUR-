using Microsoft.Extensions.Options;
using TourGuide.BLL.DTOs.Account;
using TourGuide.BLL.Options;
using TourGuide.BLL.Services.Interfaces;

namespace TourGuide.BLL.Services.Account;

// Stores files on disk under FileStorage:RootPath, outside wwwroot (NFR-02).
// Program.cs turns a relative RootPath into a path under the content root.
public class LocalFileStorage : IFileStorage
{
    private readonly string _root;

    public LocalFileStorage(IOptions<FileStorageOptions> options)
    {
        _root = Path.GetFullPath(options.Value.RootPath);
    }

    public async Task<string> SaveAsync(FileUpload file, string folder, CancellationToken ct = default)
    {
        // The extension comes from the file content, not from the user's file name.
        var contentType = FileSignatureDetector.Detect(file.Content)
            ?? throw new InvalidOperationException("The file content is not a supported type.");

        var extension = contentType switch
        {
            "application/pdf" => ".pdf",
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            _ => throw new InvalidOperationException("Unsupported file type."),
        };

        var directory = ResolveFolder(folder);
        Directory.CreateDirectory(directory);

        var storedName = $"{Guid.NewGuid():N}{extension}";
        var path = Path.Combine(directory, storedName);

        await using var target = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        if (file.Content.CanSeek)
            file.Content.Position = 0;
        await file.Content.CopyToAsync(target, ct);

        return storedName;
    }

    public Task<Stream> OpenReadAsync(string storedName, string folder, CancellationToken ct = default)
    {
        var path = ResolveFile(storedName, folder);
        if (!File.Exists(path))
            throw new FileNotFoundException("Stored file not found.", storedName);

        Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
        return Task.FromResult(stream);
    }

    public async Task<string> ReplaceAsync(string oldName, FileUpload file, string folder, CancellationToken ct = default)
    {
        var newName = await SaveAsync(file, folder, ct);
        await DeleteAsync(oldName, folder, ct);
        return newName;
    }

    public Task DeleteAsync(string storedName, string folder, CancellationToken ct = default)
    {
        var path = ResolveFile(storedName, folder);
        if (File.Exists(path))
            File.Delete(path);

        return Task.CompletedTask;
    }

    // Folder names are a fixed set (FileFolders); anything else is a programming error.
    private string ResolveFolder(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder) || folder.Any(c => !(char.IsAsciiLetterOrDigit(c) || c == '-')))
            throw new ArgumentException("Invalid storage folder.", nameof(folder));

        return Path.Combine(_root, folder);
    }

    // A stored name is a GUID plus an extension. Reject path parts so a name can never leave the folder.
    private string ResolveFile(string storedName, string folder)
    {
        if (string.IsNullOrWhiteSpace(storedName) || storedName != Path.GetFileName(storedName))
            throw new ArgumentException("Invalid stored file name.", nameof(storedName));

        return Path.Combine(ResolveFolder(folder), storedName);
    }
}
