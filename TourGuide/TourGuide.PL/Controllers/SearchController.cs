using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TourGuide.BLL.DTOs.Search;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.PL.Extensions;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Controllers;

// FR-10: public guide search (class diagram §5.4 <<Public>>). Booking still needs a Tourist login.
public class SearchController : Controller
{
    private readonly ISearchService _search;
    private readonly IMapper _mapper;

    public SearchController(ISearchService search, IMapper mapper)
    {
        _search = search;
        _mapper = mapper;
    }

    // GET /Search?CountryCode=EG&LanguageId=1&MinPrice=10&MaxPrice=50&page=2
    [HttpGet]
    public async Task<IActionResult> Index(SearchViewModel model, CancellationToken ct)
    {
        _mapper.Map(await _search.GetFilterOptionsAsync(ct), model);

        if (!ModelState.IsValid)
            return View(model);

        var result = await _search.SearchAsync(_mapper.Map<GuideSearchFilter>(model), ct);
        if (!result.Succeeded)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }

        _mapper.Map(result.Value!, model);
        return View(model);
    }
}
