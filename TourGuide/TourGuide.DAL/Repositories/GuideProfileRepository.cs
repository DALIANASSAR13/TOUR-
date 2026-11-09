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

    public async Task<PagedList<GuideProfile>> SearchAsync(GuideSearchQuery query, CancellationToken ct = default)
    {
        var guides = Set.AsNoTracking().Where(Searchable);

        if (!string.IsNullOrWhiteSpace(query.CountryCode))
        {
            var country = query.CountryCode.Trim().ToUpperInvariant();
            guides = guides.Where(g => g.CountryCode == country);
        }
        if (query.LanguageId is int languageId)
            guides = guides.Where(g => g.Languages.Any(l => l.LanguageId == languageId));
        if (query.SpecializationId is int specializationId)
            guides = guides.Where(g => g.Specializations.Any(s => s.SpecializationId == specializationId));
        if (query.MinPrice is decimal min)
            guides = guides.Where(g => g.PricePerHour >= min);
        if (query.MaxPrice is decimal max)
            guides = guides.Where(g => g.PricePerHour <= max);

        var total = await guides.CountAsync(ct);

        var items = await guides
            .OrderByDescending(g => g.AverageRating)
            .ThenByDescending(g => g.ReviewCount)
            .ThenBy(g => g.User.FullName)
            .ThenBy(g => g.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Include(g => g.User)
            .Include(g => g.Languages).ThenInclude(l => l.Language)
            .Include(g => g.Specializations).ThenInclude(s => s.Specialization)
            .AsSplitQuery()
            .ToListAsync(ct);

        return new PagedList<GuideProfile>(items, query.Page, query.PageSize, total);
    }

    public Task<List<string>> GetSearchableCountryCodesAsync(CancellationToken ct = default) =>
        Set.AsNoTracking()
            .Where(Searchable)
            .Select(g => g.CountryCode!)
            .Distinct()
            .ToListAsync(ct);
}
