using Microsoft.EntityFrameworkCore;
using TourGuide.DAL.Common;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Repositories;

// Review queries (the constructor is in SpecificRepositories.cs).
public partial class ReviewRepository
{
    public async Task<PagedList<Review>> GetForGuideAsync(int guideId, int page, int pageSize, CancellationToken ct = default)
    {
        var reviews = Set.AsNoTracking().Where(r => r.GuideProfileId == guideId);

        var total = await reviews.CountAsync(ct);

        var items = await reviews
            .OrderByDescending(r => r.CreatedAtUtc)
            .ThenByDescending(r => r.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(r => r.Tourist)
            .ToListAsync(ct);

        return new PagedList<Review>(items, page, pageSize, total);
    }
}
