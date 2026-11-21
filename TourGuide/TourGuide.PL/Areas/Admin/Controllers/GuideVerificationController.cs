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
}