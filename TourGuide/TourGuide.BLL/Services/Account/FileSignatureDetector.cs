
namespace TourGuide.BLL.Services.Account;

public static class FileSignatureDetector
{
    public static string? Detect(Stream stream)
    {
        if (!stream.CanRead)
            return null;

        if (stream.CanSeek)
            stream.Position = 0;

        var header = new byte[8];
        var count = stream.Read(header, 0, header.Length);

        if (stream.CanSeek)
            stream.Position = 0;

        if (count >= 5 &&
            header[0] == 0x25 &&
            header[1] == 0x50 &&
            header[2] == 0x44 &&
            header[3] == 0x46 &&
            header[4] == 0x2D)
            return "application/pdf";

        if (count >= 3 &&
            header[0] == 0xFF &&
            header[1] == 0xD8 &&
            header[2] == 0xFF)
            return "image/jpeg";

        if (count >= 8 &&
            header[0] == 0x89 &&
            header[1] == 0x50 &&
            header[2] == 0x4E &&
            header[3] == 0x47 &&
            header[4] == 0x0D &&
            header[5] == 0x0A &&
            header[6] == 0x1A &&
            header[7] == 0x0A)
            return "image/png";

        return null;
    }
}

