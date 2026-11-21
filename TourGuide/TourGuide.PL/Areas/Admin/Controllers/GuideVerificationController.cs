using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

    [HttpGet]
    public async Task<IActionResult> Review(int id, CancellationToken ct)
    {
        var result = await _verificationService.GetForReviewAsync(id, ct);
        if (!result.Succeeded || result.Value is null)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        var dto = result.Value;
        var viewModel = new GuideReviewViewModel
        {
            GuideProfileId = dto.GuideProfileId,
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Bio = dto.Bio,
            City = dto.City,
            CountryCode = dto.CountryCode,
            TimeZoneId = dto.TimeZoneId,
            PricePerHour = dto.PricePerHour,
            VerificationStatus = dto.VerificationStatus,
            RejectionReason = dto.RejectionReason,
            SubmittedAtUtc = dto.SubmittedAtUtc,
            Languages = dto.Languages,
            Specializations = dto.Specializations,
            Documents = dto.Documents.Select(d => new GuideDocumentReviewItemViewModel
            {
                Id = d.Id,
                DocumentType = d.DocumentType,
                OriginalFileName = d.OriginalFileName,
                ContentType = d.ContentType,
                SizeBytes = d.SizeBytes,
                UploadedAtUtc = d.UploadedAtUtc
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Document(int documentId, CancellationToken ct)
    {
        var result = await _verificationService.OpenDocumentAsync(documentId, ct);
        if (!result.Succeeded || result.Value is null)
            return NotFound();

        Response.Headers.ContentDisposition =
            $"inline; filename*=UTF-8''{Uri.EscapeDataString(result.Value.OriginalFileName)}";
        return File(result.Value.Stream, result.Value.ContentType, enableRangeProcessing: true);
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id, CancellationToken ct)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(adminId))
            return Challenge();

        var result = await _verificationService.ApproveAsync(id, adminId, ct);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "The guide application was approved."
            : result.Message;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id, string rejectionReason, CancellationToken ct)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(adminId))
            return Challenge();

        var result = await _verificationService.RejectAsync(id, adminId, rejectionReason, ct);
        if (!result.Succeeded && result.ErrorCode == TourGuide.BLL.Common.ErrorCodes.Validation)
        {
            TempData["Error"] = result.Errors.FirstOrDefault()?.Message ?? result.Message;
            return RedirectToAction(nameof(Review), new { id });
        }

        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded
            ? "The guide application was rejected."
            : result.Message;

        return RedirectToAction(nameof(Index));
    }
}