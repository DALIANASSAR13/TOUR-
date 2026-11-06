namespace TourGuide.DAL.Entities;

public class Specialization
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<GuideSpecialization> Guides { get; set; } = new List<GuideSpecialization>();
}
