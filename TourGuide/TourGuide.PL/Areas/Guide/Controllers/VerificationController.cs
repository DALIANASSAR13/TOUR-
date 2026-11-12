using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourGuide.BLL.DTOs.GuideVerification;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Constants;
using TourGuide.DAL.Enums;
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

    // GET /Guide/Verification/Reapply — FR-07 (only when Rejected)
    [HttpGet]
    public async Task<IActionResult> Reapply(CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Challenge();

        var statusResult = await _verificationService.GetMyStatusAsync(userId, ct);
        if (!statusResult.Succeeded || statusResult.Value is null)
            return NotFound();

        if (statusResult.Value.Status != VerificationStatus.Rejected)
            return RedirectToAction(nameof(Status));

        return View(new ReapplyViewModel
        {
            PreviousRejectionReason = statusResult.Value.RejectionReason
        });
    }

    // POST /Guide/Verification/Reapply — FR-07
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reapply(ReapplyViewModel vm, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Challenge();

        if (!ModelState.IsValid)
        {
            await PopulateRejectionReasonAsync(userId, vm, ct);
            return View(vm);
        }

        await using var residenceStream = vm.ProofOfResidence?.OpenReadStream();
        await using var idStream = vm.GovernmentId?.OpenReadStream();
        await using var criminalStream = vm.CriminalRecord?.OpenReadStream();

        var request = new ReapplyRequest
        {
            ProofOfResidence = vm.ProofOfResidence is null ? null : new DocumentUploadItem
            {
                Stream = residenceStream!,
                OriginalFileName = vm.ProofOfResidence.FileName,
                ContentType = vm.ProofOfResidence.ContentType,
                Length = vm.ProofOfResidence.Length
            },
            GovernmentId = vm.GovernmentId is null ? null : new DocumentUploadItem
            {
                Stream = idStream!,
                OriginalFileName = vm.GovernmentId.FileName,
                ContentType = vm.GovernmentId.ContentType,
                Length = vm.GovernmentId.Length
            },
            CriminalRecord = vm.CriminalRecord is null ? null : new DocumentUploadItem
            {
                Stream = criminalStream!,
                OriginalFileName = vm.CriminalRecord.FileName,
                ContentType = vm.CriminalRecord.ContentType,
                Length = vm.CriminalRecord.Length
            }
        };

        var result = await _verificationService.ReapplyAsync(userId, request, ct);
        if (!result.Succeeded)
        {
            if (result.Errors.Any())
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(err.Field, err.Message);
            }
            else if (!string.IsNullOrWhiteSpace(result.Message))
            {
                ModelState.AddModelError(string.Empty, result.Message);
            }

            await PopulateRejectionReasonAsync(userId, vm, ct);
            return View(vm);
        }

        TempData["ReapplySuccess"] = "Your new documents have been submitted and are now pending admin review.";
        return RedirectToAction(nameof(Status));
    }

    private async Task PopulateRejectionReasonAsync(string userId, ReapplyViewModel vm, CancellationToken ct)
    {
        var status = await _verificationService.GetMyStatusAsync(userId, ct);
        if (status.Succeeded && status.Value is not null)
            vm.PreviousRejectionReason = status.Value.RejectionReason;
    }
}