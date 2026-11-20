using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.PL.Controllers.Base;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Areas.Admin.Controllers;

public class GuideVerificationController : AdminBaseController
{
    private readonly IGuideVerificationService _verificationService;
    private readonly IMapper _mapper;

    public GuideVerificationController(
        IGuideVerificationService verificationService,
        IMapper mapper)
    {
        _verificationService = verificationService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var result = await _verificationService.GetPendingAsync(ct);
        if (!result.Succeeded || result.Value is null)
        {
            TempData["Error"] = result.Message;
            return View(new PendingGuidesViewModel());
        }

        var viewModel = new PendingGuidesViewModel
        {
            Guides = _mapper.Map<IReadOnlyList<PendingGuideItemViewModel>>(result.Value)
        };

        return View(viewModel);
    }
}