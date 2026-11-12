using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TourGuide.DAL.Common;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Enums;
using TourGuide.DAL.Queries;

namespace TourGuide.DAL.Repositories;

// Guide profile queries (the constructor is in SpecificRepositories.cs).
public partial class GuideProfileRepository
{
    // BR-02 as a query filter (Class Specs §2 "Profile complete"). Same condition as BLL GuideRules.IsBookable,
    // which checks a loaded entity; the class diagram puts the search filter here (§5.4).
    private static readonly Expression<Func<GuideProfile, bool>> Searchable = g =>
        g.VerificationStatus == VerificationStatus.Approved
        && g.CountryCode != null && g.CountryCode != ""
        && g.City != null && g.City != ""
        && g.TimeZoneId != null && g.TimeZoneId != ""
        && g.PricePerHour > 0
        && g.Languages.Any()
        && g.Specializations.Any();

    public Task<GuideProfile?> GetWithDetailsAsync(int id, CancellationToken ct = default) =>
        Set.AsNoTracking()
            .Include(g => g.User)
            .Include(g => g.Languages).ThenInclude(l => l.Language)
            .Include(g => g.Specializations).ThenInclude(s => s.Specialization)
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
