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

    public async Task<PagedList<GuideProfile>> SearchAsync(GuideSearchQuery query, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var page = Math.Max(1, query.Page);
        var pageSize = Math.Max(1, query.PageSize);

        IQueryable<GuideProfile> profiles = Set.AsNoTracking()
            .Where(Searchable);

        if (!string.IsNullOrWhiteSpace(query.CountryCode))
        {
            var countryCode = query.CountryCode.Trim().ToUpperInvariant();
            profiles = profiles.Where(g => g.CountryCode == countryCode);
        }

        if (query.LanguageId.HasValue)
            profiles = profiles.Where(g => g.Languages.Any(l => l.LanguageId == query.LanguageId.Value));

        if (query.SpecializationId.HasValue)
            profiles = profiles.Where(g => g.Specializations.Any(s => s.SpecializationId == query.SpecializationId.Value));

        if (query.MinPrice.HasValue)
            profiles = profiles.Where(g => g.PricePerHour >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            profiles = profiles.Where(g => g.PricePerHour <= query.MaxPrice.Value);

        var totalCount = await profiles.CountAsync(ct);

        var items = await profiles
            .Include(g => g.User)
            .Include(g => g.Languages).ThenInclude(l => l.Language)
            .Include(g => g.Specializations).ThenInclude(s => s.Specialization)
            .AsSplitQuery()
            .OrderByDescending(g => g.AverageRating)
            .ThenByDescending(g => g.ReviewCount)
            .ThenBy(g => g.User.FullName)
            .ThenBy(g => g.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedList<GuideProfile>(items, page, pageSize, totalCount);
    }

    public Task<List<string>> GetSearchableCountryCodesAsync(CancellationToken ct = default) =>
        Set.AsNoTracking()
            .Where(Searchable)
            .Select(g => g.CountryCode!)
            .Distinct()
            .OrderBy(code => code)
            .ToListAsync(ct);

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
