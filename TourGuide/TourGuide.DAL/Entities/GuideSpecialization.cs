namespace TourGuide.DAL.Entities;

public class GuideSpecialization
{
    public int GuideProfileId { get; set; }
    public int SpecializationId { get; set; }

    public GuideProfile GuideProfile { get; set; } = null!;
    public Specialization Specialization { get; set; } = null!;
}
