using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.Search;
using TourGuide.DAL.Common;

namespace TourGuide.BLL.Services.Interfaces;

// FR-10: search approved guides with a complete profile (BR-02).
public interface ISearchService
{
    // Validates the filter first; one page of guide cards, best rated first.
    Task<Result<PagedList<GuideCardDto>>> SearchAsync(GuideSearchFilter filter, CancellationToken ct = default);

    // Dropdown choices: countries that have searchable guides, all languages and specializations.
    Task<SearchFilterOptionsDto> GetFilterOptionsAsync(CancellationToken ct = default);
}
