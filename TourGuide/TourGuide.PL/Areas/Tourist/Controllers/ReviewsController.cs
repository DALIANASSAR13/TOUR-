using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.Review;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Constants;
using TourGuide.PL.Controllers.Base;
using TourGuide.PL.Extensions;
using TourGuide.PL.Services;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Areas.Tourist.Controllers;

[Area("Tourist")]
[Authorize(Roles = Roles.Tourist)] // NFR-01
public class ReviewsController : TouristBaseController
{
    private readonly IReviewService _reviews;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public ReviewsController(IReviewService reviews, ICurrentUserService currentUser, IMapper mapper)
    {
        _reviews = reviews;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    // GET /Tourist/Reviews/Create/5 — FR-19. 5 = booking id. Only the owner's Completed, unreviewed booking
    // (BR-13, BR-14, BR-15); anything else goes back to My Bookings. Saves nothing (NFR-05).
    [HttpGet]
    public async Task<IActionResult> Create(int id, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Challenge();

        if (!await _reviews.CanReviewAsync(userId, id, ct))
        {
            TempData["BookingError"] = "This booking cannot be reviewed.";
            return RedirectToAction("Index", "Bookings");
        }

        return View(new CreateReviewViewModel { BookingId = id });
    }

    // POST /Tourist/Reviews/Create — FR-19, FR-20. PRG back to My Bookings.
    [HttpPost]
    public async Task<IActionResult> Create(CreateReviewViewModel model, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Challenge();

        if (!ModelState.IsValid)
            return View(model);

        var result = await _reviews.AddAsync(userId, _mapper.Map<CreateReviewRequest>(model), ct);
        if (result.Succeeded)
        {
            TempData["BookingMessage"] = "Thank you for your review.";
            return RedirectToAction("Index", "Bookings");
        }

        // Field errors stay on the form; a refused rule (not found, not completed, already reviewed)
        // goes back to My Bookings with its message.
        if (result.ErrorCode == ErrorCodes.Validation)
        {
            ModelState.AddResultErrors(result);
            return View(model);
        }

        TempData["BookingError"] = result.Message;
        return RedirectToAction("Index", "Bookings");
    }
}
