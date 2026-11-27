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
public class PaymentsController : Controller
{
    private readonly IPaymentService _payments;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public PaymentsController(IPaymentService payments, ICurrentUserService currentUser, IMapper mapper)
    {
        _payments = payments;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    // GET /Tourist/Payments/Checkout/5 — FR-14. Read-only (NFR-05); a refused booking goes back to My Bookings.
    [HttpGet]
    public async Task<IActionResult> Checkout(int id, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Challenge();

        var result = await _payments.GetCheckoutAsync(userId, id, ct);
        if (!result.Succeeded)
        {
            TempData["BookingError"] = result.Message;
            return RedirectToAction("Index", "Bookings");
        }

        return View(_mapper.Map<CheckoutViewModel>(result.Value));
    }
}
