using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Constants;
using TourGuide.PL.Services;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Controllers;

// FR-09: public guide profile (class diagram <<Public>>). Booking itself needs the Tourist role (2.6).
public class GuidesController : Controller
{
    private readonly IGuideProfileService _guides;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GuidesController(IGuideProfileService guides, ICurrentUserService currentUser, IMapper mapper)
    {
        _guides = guides;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    // GET /Guides/Details/5?page=2 — page = reviews page
    [HttpGet]
    public async Task<IActionResult> Details(int id, int page = 1, CancellationToken ct = default)
    {
        var result = await _guides.GetPublicProfileAsync(id, page, ct);
        if (!result.Succeeded)
            return NotFound(); // BR-02: not approved, incomplete or unknown

        var model = _mapper.Map<PublicGuideProfileViewModel>(result.Value);
        model.CanBook = !_currentUser.IsInRole(Roles.Guide) && !_currentUser.IsInRole(Roles.Admin);
        return View(model);
    }
}
