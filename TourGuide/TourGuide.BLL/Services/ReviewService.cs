using AutoMapper;
using FluentValidation;
using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.Review;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Enums;
using TourGuide.DAL.UnitOfWork;

namespace TourGuide.BLL.Services;

public class ReviewService : IReviewService
{
    private const int AverageDecimals = 2; // GuideProfile.AverageRating is decimal(3,2)

    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateReviewRequest> _validator;
    private readonly TimeProvider _time;

    public ReviewService(IUnitOfWork uow, IMapper mapper, IValidator<CreateReviewRequest> validator, TimeProvider time)
    {
        _uow = uow;
        _mapper = mapper;
        _validator = validator;
        _time = time;
    }

    // BR-13 own booking, BR-14 Completed, BR-15 no review yet. Read-only.
    public async Task<bool> CanReviewAsync(string touristId, int bookingId, CancellationToken ct = default)
    {
        var booking = await _uow.Bookings.GetByIdAsync(bookingId, ct);
        if (booking is null || booking.TouristId != touristId || booking.Status != BookingStatus.Completed)
            return false;

        return !await _uow.Reviews.ExistsForBookingAsync(bookingId, ct);
    }

    // FR-19, FR-20. Order: validator -> BR-13 -> BR-14 -> BR-15 -> one save.
    // The review and the guide's new rating are saved by a single SaveChangesAsync, which is one
    // database transaction, so a review never exists without its rating update (and the other way round).
    public async Task<Result> AddAsync(string touristId, CreateReviewRequest request, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Invalid(validation.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToList());

        var booking = await _uow.Bookings.GetByIdAsync(request.BookingId, ct);
        if (booking is null || booking.TouristId != touristId)                       // BR-13
            return Result.Fail(ErrorCodes.NotFound, "This booking was not found.");

        if (booking.Status != BookingStatus.Completed)                               // BR-14
            return Result.Fail(ErrorCodes.InvalidStatusChange,
                "You can review a trip only after it is completed.");

        if (await _uow.Reviews.ExistsForBookingAsync(booking.Id, ct))                // BR-15
            return Result.Fail(ErrorCodes.AlreadyReviewed, "You have already reviewed this booking.");

        var guide = await _uow.GuideProfiles.GetByIdAsync(booking.GuideProfileId, ct); // tracked
        if (guide is null)
            return Result.Fail(ErrorCodes.NotFound, "This guide was not found.");

        var review = _mapper.Map<Review>(request);
        review.TouristId = touristId;
        review.GuideProfileId = guide.Id;
        review.Comment = string.IsNullOrWhiteSpace(review.Comment) ? null : review.Comment.Trim();
        review.CreatedAtUtc = _time.GetUtcNow().UtcDateTime;
        await _uow.Reviews.AddAsync(review, ct);

        // FR-20: new average from the stored average and count (decimal, rounded once).
        var totalStars = guide.AverageRating * guide.ReviewCount + review.Rating;
        guide.ReviewCount += 1;
        guide.AverageRating = Math.Round(totalStars / guide.ReviewCount, AverageDecimals, MidpointRounding.AwayFromZero);

        await _uow.SaveChangesAsync(ct);
        return Result.Ok();
    }
}
