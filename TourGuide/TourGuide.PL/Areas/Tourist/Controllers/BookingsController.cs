using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourGuide.BLL.Common;
using TourGuide.BLL.Domain;
using TourGuide.BLL.DTOs.Booking;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Constants;
using TourGuide.DAL.Enums;
using TourGuide.PL.Controllers.Base;
using TourGuide.PL.Extensions;
using TourGuide.PL.Services;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Areas.Tourist.Controllers;

[Area("Tourist")]
[Authorize(Roles = Roles.Tourist)] // NFR-01
public class BookingsController : TouristBaseController
{
    private readonly IBookingService _bookings;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly TimeProvider _time;

    public BookingsController(IBookingService bookings, ICurrentUserService currentUser, IMapper mapper, TimeProvider time)
    {
        _bookings = bookings;
        _currentUser = currentUser;
        _mapper = mapper;
        _time = time;
    }

    // GET /Tourist/Bookings — FR-15
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Challenge();

        var bookings = await _bookings.GetForTouristAsync(userId, ct);

        return View(new MyBookingsViewModel
        {
            Bookings = _mapper.Map<List<BookingItemViewModel>>(bookings),
            NowUtc = _time.GetUtcNow().UtcDateTime,
        });
    }

    // GET /Tourist/Bookings/Create/5 or /Tourist/Bookings/Create?guideProfileId=5 — FR-11 form with the FR-12 price box.
    // The first quote also checks the guide is bookable (BR-02).
    [HttpGet("Tourist/Bookings/Create/{guideProfileId:int?}")]
    public async Task<IActionResult> Create(int guideProfileId, CancellationToken ct)
    {
        var model = new CreateBookingViewModel
        {
            GuideProfileId = guideProfileId,
            BookingType = BookingType.Hourly,
            Hours = BookingRules.MinHours,
        };

        var result = await _bookings.PreviewPriceAsync(_mapper.Map<CreateBookingRequest>(model), ct);
        if (!result.Succeeded)
            return NotFound();

        model.GuideName = result.Value!.GuideName;
        model.Quote = _mapper.Map<PriceQuoteViewModel>(result.Value);
        return View(model);
    }

    // POST /Tourist/Bookings/Create — FR-11. Saves a Pending request; PRG back to My Bookings.
    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingViewModel model, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Challenge();

        if (!ModelState.IsValid)
            return await RedrawFormAsync(model, ct);

        var result = await _bookings.CreateAsync(userId, _mapper.Map<CreateBookingRequest>(model), ct);
        if (!result.Succeeded)
        {
            ModelState.AddResultErrors(result);
            return await RedrawFormAsync(model, ct);
        }

        TempData["BookingMessage"] =
            $"Request sent to {model.GuideName}. The guide has {BookingRules.PendingExpiryHours} hours to answer.";
        return RedirectToAction(nameof(Index));
    }

    // POST /Tourist/Bookings/PreviewPrice (AJAX) — FR-12. Saves nothing (NFR-05).
    [HttpPost]
    public async Task<IActionResult> PreviewPrice(CreateBookingViewModel model, CancellationToken ct)
    {
        // The start is not needed for a price.
        ModelState.Remove(nameof(CreateBookingViewModel.StartDate));
        ModelState.Remove(nameof(CreateBookingViewModel.StartTime));

        if (!ModelState.IsValid)
            return BadRequest(new { errors = ModelState.ToErrorDictionary() });

        var result = await _bookings.PreviewPriceAsync(_mapper.Map<CreateBookingRequest>(model), ct);
        if (!result.Succeeded)
        {
            ModelState.AddResultErrors(result);
            var body = new { message = result.Message, errors = ModelState.ToErrorDictionary() };
            return result.ErrorCode == ErrorCodes.Validation ? BadRequest(body) : NotFound(body);
        }

        return Json(_mapper.Map<PriceQuoteViewModel>(result.Value));
    }

    // Same form with the errors; the price box is refreshed when the amounts are valid.
    private async Task<IActionResult> RedrawFormAsync(CreateBookingViewModel model, CancellationToken ct)
    {
        var quote = await _bookings.PreviewPriceAsync(_mapper.Map<CreateBookingRequest>(model), ct);
        if (quote.Succeeded)
        {
            model.GuideName = quote.Value!.GuideName;
            model.Quote = _mapper.Map<PriceQuoteViewModel>(quote.Value);
        }
        return View(nameof(Create), model);
    }
}
