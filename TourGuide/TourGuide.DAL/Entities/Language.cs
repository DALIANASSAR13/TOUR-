namespace TourGuide.DAL.Entities;

public class Language
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<GuideLanguage> Guides { get; set; } = new List<GuideLanguage>();
}
