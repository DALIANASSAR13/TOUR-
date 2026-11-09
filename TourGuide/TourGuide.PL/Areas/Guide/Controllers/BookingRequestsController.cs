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
public class BookingRequestsController : Controller
{
    private readonly IBookingService _bookings;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly TimeProvider _time;

    public BookingRequestsController(IBookingService bookings, ICurrentUserService currentUser, IMapper mapper, TimeProvider time)
    {
        _bookings = bookings;
        _currentUser = currentUser;
        _mapper = mapper;
        _time = time;
    }

    // GET /Guide/BookingRequests — FR-16
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Challenge();

        var result = await _bookings.GetForGuideAsync(userId, ct);

        return View(new GuideDashboardViewModel
        {
            Bookings = result.Succeeded ? _mapper.Map<List<GuideBookingItemViewModel>>(result.Value) : new(),
            Notice = result.Succeeded ? null : result.Message,
            NowUtc = _time.GetUtcNow().UtcDateTime,
        });
    }

    // POST /Guide/BookingRequests/Accept/5 — FR-13. PRG back to the list with a message.
    [HttpPost]
    public async Task<IActionResult> Accept(int id, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Challenge();

        var result = await _bookings.AcceptAsync(userId, id, ct);
        if (result.Succeeded)
            TempData["BookingMessage"] = $"Request #{BookingItemViewModel.RefOf(id)} accepted. The tourist must pay before the deadline shown on the card.";
        else
            TempData["BookingError"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    // POST /Guide/BookingRequests/Reject/5 — FR-13. PRG back to the list with a message.
    [HttpPost]
    public async Task<IActionResult> Reject(int id, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Challenge();

        var result = await _bookings.RejectAsync(userId, id, ct);
        if (result.Succeeded)
            TempData["BookingMessage"] = $"Request #{BookingItemViewModel.RefOf(id)} rejected.";
        else
            TempData["BookingError"] = result.Message;

        return RedirectToAction(nameof(Index));
    }
}
