using AutoMapper;
using FluentValidation;
using TourGuide.BLL.Common;
using TourGuide.BLL.Domain;
using TourGuide.BLL.Domain.Interfaces;
using TourGuide.BLL.DTOs.Booking;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Enums;
using TourGuide.DAL.UnitOfWork;

namespace TourGuide.BLL.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IPriceCalculator _price;
    private readonly IValidator<CreateBookingRequest> _createValidator;

    public BookingService(IUnitOfWork uow, IMapper mapper, IPriceCalculator price,
        IValidator<CreateBookingRequest> createValidator)
    {
        _uow = uow;
        _mapper = mapper;
        _price = price;
        _createValidator = createValidator;
    }

    // FR-12, BR-11: price before sending. BR-02: only a bookable guide. Read-only (NFR-05).
    public async Task<Result<PriceQuoteDto>> PreviewPriceAsync(CreateBookingRequest request, CancellationToken ct = default)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result<PriceQuoteDto>.Invalid(validation.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToList());

        var guide = await _uow.GuideProfiles.GetWithDetailsAsync(request.GuideProfileId, ct);
        if (guide is null || !GuideRules.IsBookable(guide))
            return Result<PriceQuoteDto>.Fail(ErrorCodes.GuideNotBookable, "This guide cannot be booked right now.");

        var pricePerHour = guide.PricePerHour!.Value;
        var billable = _price.BillableHours(request.BookingType, request.Hours, request.Days);

        return Result<PriceQuoteDto>.Ok(new PriceQuoteDto
        {
            GuideProfileId = guide.Id,
            GuideName = guide.User.FullName,
            BookingType = request.BookingType,
            Hours = request.Hours,
            Days = request.Days,
            BillableHours = billable,
            PricePerHour = pricePerHour,
            TotalPrice = _price.Total(billable, pricePerHour),
        });
    }

    // FR-15, NFR-01: only this tourist's bookings. Read-only: no status changes here (NFR-05).
    public async Task<List<BookingDto>> GetForTouristAsync(string touristId, CancellationToken ct = default)
    {
        var bookings = await _uow.Bookings.GetForTouristAsync(touristId, ct);
        var dtos = _mapper.Map<List<BookingDto>>(bookings);
        dtos.ForEach(SetAllowedActions);
        return dtos;
    }

    // Which actions the tourist may start. The services of 2.8.b / 2.9 / 3.2 check these again.
    private static void SetAllowedActions(BookingDto b)
    {
        b.AutoCancelAtUtc = b.Status == BookingStatus.Pending
            ? b.CreatedAtUtc.AddHours(BookingRules.PendingExpiryHours)          // BR-06
            : null;
        b.CanPay = b.Status == BookingStatus.Accepted;                          // BR-07
        b.CanWithdraw = b.Status is BookingStatus.Pending or BookingStatus.Accepted; // BR-07
        b.CanReview = b.Status == BookingStatus.Completed && b.ReviewRating is null; // BR-13, BR-15
    }
}
