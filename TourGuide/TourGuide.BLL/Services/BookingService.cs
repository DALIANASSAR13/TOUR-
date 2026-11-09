using AutoMapper;
using FluentValidation;
using TourGuide.BLL.Common;
using TourGuide.BLL.Domain;
using TourGuide.BLL.Domain.Interfaces;
using TourGuide.BLL.DTOs.Booking;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.BLL.Validators;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Enums;
using TourGuide.DAL.Exceptions;
using TourGuide.DAL.UnitOfWork;

namespace TourGuide.BLL.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IPriceCalculator _price;
    private readonly IValidator<CreateBookingRequest> _createValidator;
    private readonly IBookingStateMachine _stateMachine;
    private readonly TimeProvider _time;

    public BookingService(IUnitOfWork uow, IMapper mapper, IPriceCalculator price,
        IValidator<CreateBookingRequest> createValidator, IBookingStateMachine stateMachine, TimeProvider time)
    {
        _uow = uow;
        _mapper = mapper;
        _price = price;
        _createValidator = createValidator;
        _stateMachine = stateMachine;
        _time = time;
    }

    // FR-11: BR-04 (validator, "Create" rule set), BR-02 (bookable guide), BR-11 (price copied now).
    // The guide-local start is stored in UTC (Class Specs §5). New requests start as Pending.
    public async Task<Result<BookingDto>> CreateAsync(string touristId, CreateBookingRequest request, CancellationToken ct = default)
    {
        var validation = await _createValidator.ValidateAsync(request,
            o => o.IncludeRulesNotInRuleSet().IncludeRuleSets(CreateBookingValidator.CreateRuleSet), ct);
        if (!validation.IsValid)
            return Result<BookingDto>.Invalid(validation.Errors
                .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToList());

        var guide = await _uow.GuideProfiles.GetWithDetailsAsync(request.GuideProfileId, ct);
        if (guide is null || !GuideRules.IsBookable(guide))
            return Result<BookingDto>.Fail(ErrorCodes.GuideNotBookable, "This guide cannot be booked right now.");

        var times = BookingTimes.Resolve(request, BookingTimes.FindZone(guide.TimeZoneId!));
        if (times is not { } span)
            return Result<BookingDto>.Invalid(new List<ValidationError>
            {
                new(nameof(CreateBookingRequest.StartTime), "Choose a valid start time."),
            });

        var booking = _mapper.Map<Booking>(request);
        booking.TouristId = touristId;
        booking.StartUtc = span.StartUtc;
        booking.EndUtc = span.EndUtc;
        booking.BillableHours = _price.BillableHours(request.BookingType, request.Hours, request.Days);
        booking.PricePerHourAtRequest = guide.PricePerHour!.Value;
        booking.TotalPrice = _price.Total(booking.BillableHours, booking.PricePerHourAtRequest);
        booking.Status = BookingStatus.Pending;
        booking.CreatedAtUtc = _time.GetUtcNow().UtcDateTime;

        await _uow.Bookings.AddAsync(booking, ct);
        await _uow.SaveChangesAsync(ct);

        var dto = _mapper.Map<BookingDto>(booking);
        SetAllowedActions(dto);
        return Result<BookingDto>.Ok(dto);
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

    // FR-16, NFR-01: only bookings made to this guide's profile. BR-01: an unapproved guide gets none.
    // Read-only: no status changes here (NFR-05).
    public async Task<Result<List<BookingDto>>> GetForGuideAsync(string guideUserId, CancellationToken ct = default)
    {
        var guide = await _uow.GuideProfiles.GetByUserIdAsync(guideUserId, ct);
        if (guide is null)
            return Result<List<BookingDto>>.Fail(ErrorCodes.NotFound, "No guide profile was found for this account.");
        if (guide.VerificationStatus != VerificationStatus.Approved)
            return Result<List<BookingDto>>.Fail(ErrorCodes.GuideNotApproved,
                "Your account is not approved yet. Requests will appear here after an admin approves you.");

        var bookings = await _uow.Bookings.GetForGuideAsync(guide.Id, ct);
        var dtos = _mapper.Map<List<BookingDto>>(bookings);
        dtos.ForEach(SetAllowedActions);

        // BR-09 shown early: warn on Pending rows that clash with a blocking booking (same list, no extra query).
        foreach (var pending in dtos.Where(d => d.Status == BookingStatus.Pending))
            pending.OverlapsWithBookingId = dtos.FirstOrDefault(o => o.Id != pending.Id
                && BookingOverlap.Blocks(o.Status)
                && BookingOverlap.Overlaps(pending.StartUtc, pending.EndUtc, o.StartUtc, o.EndUtc))?.Id;

        return Result<List<BookingDto>>.Ok(dtos);
    }

    // FR-13: Pending -> Accepted by the owner guide. BR-01 approved guide, NFR-01 owner only,
    // BR-06 not expired, BR-09 no overlap, BR-08 deadline (set by the state machine), RowVersion on save.
    public async Task<Result> AcceptAsync(string guideUserId, int bookingId, CancellationToken ct = default)
    {
        var guide = await _uow.GuideProfiles.GetByUserIdAsync(guideUserId, ct);
        if (guide is null || guide.VerificationStatus != VerificationStatus.Approved)
            return Result.Fail(ErrorCodes.GuideNotApproved, "Your account is not approved yet.");

        var booking = await _uow.Bookings.GetByIdAsync(bookingId, ct);
        if (booking is null || booking.GuideProfileId != guide.Id)
            return Result.Fail(ErrorCodes.NotFound, "This request was not found.");

        if (!_stateMachine.CanChange(booking.Status, BookingStatus.Accepted, Actor.Guide))
            return Result.Fail(ErrorCodes.InvalidStatusChange,
                $"This request is already {booking.Status.ToString().ToLowerInvariant()} and cannot be accepted.");

        var now = _time.GetUtcNow().UtcDateTime;
        if (booking.CreatedAtUtc.AddHours(BookingRules.PendingExpiryHours) <= now)             // BR-06
            return Result.Fail(ErrorCodes.InvalidStatusChange,
                $"This request has expired. Requests must be answered within {BookingRules.PendingExpiryHours} hours.");

        if (await _uow.Bookings.HasOverlapAsync(guide.Id, booking.StartUtc, booking.EndUtc, booking.Id, ct)) // BR-09
            return Result.Fail(ErrorCodes.Overlap,
                "This request overlaps another booking you have accepted or confirmed.");

        var change = _stateMachine.Apply(booking, BookingStatus.Accepted, Actor.Guide, now);  // BR-05, BR-08
        if (!change.Succeeded)
            return change;

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (ConcurrencyException)
        {
            return Result.Fail(ErrorCodes.Concurrency,
                "This booking was changed by someone else. Refresh the page and try again.");
        }

        return Result.Ok();
    }

    // Which actions the tourist or guide may start. The services of 2.7.b/c, 2.8.b, 2.9, 3.2 check these again.
    private static void SetAllowedActions(BookingDto b)
    {
        b.AutoCancelAtUtc = b.Status == BookingStatus.Pending
            ? b.CreatedAtUtc.AddHours(BookingRules.PendingExpiryHours)          // BR-06
            : null;
        b.CanPay = b.Status == BookingStatus.Accepted;                          // BR-07
        b.CanWithdraw = b.Status is BookingStatus.Pending or BookingStatus.Accepted; // BR-07
        b.CanReview = b.Status == BookingStatus.Completed && b.ReviewRating is null; // BR-13, BR-15
        b.CanAccept = b.Status == BookingStatus.Pending;                        // BR-06
        b.CanReject = b.Status == BookingStatus.Pending;                        // BR-06
    }
}
