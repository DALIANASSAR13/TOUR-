using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Data.Seed;

// Seeded lookup lists for the search filters (FR-08, FR-10). Not editable in the MVP.
// Specializations come from the TourType values the team used before; languages are a starting list.
// Both lists still need the team's confirmation (STATE.md -> Known issues).
public static class LookupSeed
{
    public static readonly Language[] Languages =
    {
        new() { Id = 1, Name = "English" },
        new() { Id = 2, Name = "Arabic" },
        new() { Id = 3, Name = "French" },
        new() { Id = 4, Name = "German" },
        new() { Id = 5, Name = "Spanish" },
        new() { Id = 6, Name = "Italian" },
        new() { Id = 7, Name = "Russian" },
        new() { Id = 8, Name = "Chinese" },
        new() { Id = 9, Name = "Japanese" },
    };

    public static readonly Specialization[] Specializations =
    {
        new() { Id = 1, Name = "Historical" },
        new() { Id = 2, Name = "Camping" },
        new() { Id = 3, Name = "Cruise" },
        new() { Id = 4, Name = "Walking" },
        new() { Id = 5, Name = "Photography" },
    };
}
