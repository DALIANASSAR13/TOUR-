namespace TourGuide.DAL.Entities;

public class GuideLanguage
{
    public int GuideProfileId { get; set; }
    public int LanguageId { get; set; }

    public GuideProfile GuideProfile { get; set; } = null!;
    public Language Language { get; set; } = null!;
}
