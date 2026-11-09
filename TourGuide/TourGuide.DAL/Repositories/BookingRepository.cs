using Microsoft.EntityFrameworkCore;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Enums;

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

    public Task<List<Booking>> GetForGuideAsync(int guideId, CancellationToken ct = default) =>
        Set.AsNoTracking()
            .Where(b => b.GuideProfileId == guideId)
            .Include(b => b.Tourist)
            .Include(b => b.GuideProfile)
            .Include(b => b.Payment)
            .Include(b => b.Review)
            .OrderByDescending(b => b.CreatedAtUtc)
            .ToListAsync(ct);

    // Overlap: A.StartUtc < B.EndUtc && B.StartUtc < A.EndUtc (Class Specs §5).
    public Task<bool> HasOverlapAsync(int guideId, DateTime startUtc, DateTime endUtc, int? excludeId, CancellationToken ct = default) =>
        Set.AsNoTracking().AnyAsync(b =>
            b.GuideProfileId == guideId
            && (b.Status == BookingStatus.Accepted || b.Status == BookingStatus.Confirmed)
            && (excludeId == null || b.Id != excludeId)
            && b.StartUtc < endUtc && startUtc < b.EndUtc, ct);
}
