using Microsoft.EntityFrameworkCore;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Repositories;

// Guide profile queries (the constructor is in SpecificRepositories.cs).
public partial class GuideProfileRepository
{
    public Task<GuideProfile?> GetWithDetailsAsync(int id, CancellationToken ct = default) =>
        Set.AsNoTracking()
            .Include(g => g.User)
            .Include(g => g.Languages)
            .Include(g => g.Specializations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public Task<GuideProfile?> GetByUserIdAsync(string userId, CancellationToken ct = default) =>
        Set.AsNoTracking().FirstOrDefaultAsync(g => g.UserId == userId, ct);

    public Task<GuideProfile?> GetByUserIdForUpdateAsync(string userId, CancellationToken ct = default) =>
        Set.Include(g => g.User)
            .Include(g => g.Languages)
            .Include(g => g.Specializations)
            .AsSplitQuery()
            .FirstOrDefaultAsync(g => g.UserId == userId, ct);
}