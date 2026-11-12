using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Constants;
using TourGuide.PL.Services;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Areas.Guide.Controllers;

[Area("Guide")]
[Authorize(Roles = Roles.Guide)] // NFR-01
public class VerificationController : Controller
{
    private readonly IGuideVerificationService _verificationService;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public VerificationController(
        IGuideVerificationService verificationService,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _verificationService = verificationService;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    // GET /Guide/Verification/Status — FR-07
    [HttpGet]
    public async Task<IActionResult> Status(CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Challenge();

        var result = await _verificationService.GetMyStatusAsync(userId, ct);
        if (!result.Succeeded || result.Value is null)
            return NotFound();

        var vm = _mapper.Map<GuideStatusViewModel>(result.Value);
        return View(vm);
    }
}