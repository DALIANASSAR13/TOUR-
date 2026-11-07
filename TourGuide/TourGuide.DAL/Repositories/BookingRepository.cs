using Microsoft.EntityFrameworkCore;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Repositories;

// Booking queries (the constructor is in SpecificRepositories.cs).
public partial class BookingRepository
{
    public Task<List<Booking>> GetForTouristAsync(string touristId, CancellationToken ct = default) =>
        Set.AsNoTracking()
            .Where(b => b.TouristId == touristId)
            .Include(b => b.GuideProfile).ThenInclude(g => g.User)
            .Include(b => b.Payment)
            .Include(b => b.Review)
            .OrderByDescending(b => b.CreatedAtUtc)
            .ToListAsync(ct);
}
