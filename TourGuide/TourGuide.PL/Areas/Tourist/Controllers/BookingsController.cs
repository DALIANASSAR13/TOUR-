using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Constants;
using TourGuide.PL.Services;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Areas.Tourist.Controllers;

[Area("Tourist")]
[Authorize(Roles = Roles.Tourist)] // NFR-01
public class BookingsController : Controller
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
}
