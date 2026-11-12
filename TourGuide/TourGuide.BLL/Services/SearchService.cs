using AutoMapper;
using FluentValidation;
using TourGuide.BLL.Common;
using TourGuide.BLL.Domain;
using TourGuide.BLL.DTOs.Search;
using TourGuide.BLL.Infrastructure;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Common;
using TourGuide.DAL.Queries;
using TourGuide.DAL.UnitOfWork;

namespace TourGuide.BLL.Services;

public class SearchService : ISearchService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<GuideSearchFilter> _validator;
    private readonly ICountryProvider _countries;

    public SearchService(IUnitOfWork uow, IMapper mapper, IValidator<GuideSearchFilter> validator, ICountryProvider countries)
    {
        _uow = uow;
        _mapper = mapper;
        _validator = validator;
        _countries = countries;
    }

    // FR-10. BR-02 is applied by the repository on every call. Read-only.
    public async Task<Result<PagedList<GuideCardDto>>> SearchAsync(GuideSearchFilter filter, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(filter, ct);
        if (!validation.IsValid)
            return Result<PagedList<GuideCardDto>>.Invalid(validation.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToList());

        var query = _mapper.Map<GuideSearchQuery>(filter);
        query.PageSize = SearchRules.PageSize;

        var page = await _uow.GuideProfiles.SearchAsync(query, ct);

        var cards = _mapper.Map<List<GuideCardDto>>(page.Items);
        foreach (var card in cards)
            card.CountryName = _countries.GetName(card.CountryCode);

        return Result<PagedList<GuideCardDto>>.Ok(new PagedList<GuideCardDto>(cards, page.Page, page.PageSize, page.TotalCount));
    }

    public async Task<SearchFilterOptionsDto> GetFilterOptionsAsync(CancellationToken ct = default)
    {
        var used = new HashSet<string>(await _uow.GuideProfiles.GetSearchableCountryCodesAsync(ct), StringComparer.OrdinalIgnoreCase);
        var languages = await _uow.Languages.FindAsync(_ => true, ct);
        var specializations = await _uow.Specializations.FindAsync(_ => true, ct);

        return new SearchFilterOptionsDto
        {
            Countries = _countries.GetAll().Where(c => used.Contains(c.Code)).ToList(),
            Languages = _mapper.Map<List<LookupDto>>(languages.OrderBy(l => l.Name)),
            Specializations = _mapper.Map<List<LookupDto>>(specializations.OrderBy(s => s.Name)),
        };
    }
}
