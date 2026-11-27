using AutoMapper;
using TourGuide.BLL.Common;
using TourGuide.BLL.Domain;
using TourGuide.BLL.Domain.Interfaces;
using TourGuide.BLL.DTOs.Payment;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Enums;
using TourGuide.DAL.UnitOfWork;

namespace TourGuide.BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IBookingStateMachine _stateMachine;
    private readonly TimeProvider _time;

    public PaymentService(IUnitOfWork uow, IMapper mapper, IBookingStateMachine stateMachine, TimeProvider time)
    {
        _uow = uow;
        _mapper = mapper;
        _stateMachine = stateMachine;
        _time = time;
    }

    // FR-14 checkout. NFR-01 owner only, BR-07 payable status only (a withdrawn booking is refused),
    // BR-08 before the deadline, one payment per booking. Read-only: no status change, no save (NFR-05).
    public async Task<Result<CheckoutDto>> GetCheckoutAsync(string touristId, int bookingId, CancellationToken ct = default)
    {
        var booking = await _uow.Bookings.GetWithGuideAsync(bookingId, ct);
        if (booking is null || booking.TouristId != touristId)
            return Result<CheckoutDto>.Fail(ErrorCodes.NotFound, "This booking was not found.");

        if (!_stateMachine.CanChange(booking.Status, BookingStatus.Confirmed, Actor.Tourist))
            return Result<CheckoutDto>.Fail(ErrorCodes.InvalidStatusChange, booking.Status switch
            {
                BookingStatus.Pending => "The guide has not accepted this booking yet.",
                BookingStatus.Confirmed or BookingStatus.Completed => "This booking is already paid.",
                _ => $"This booking is {booking.Status.ToString().ToLowerInvariant()} and cannot be paid.",
            });

        var now = _time.GetUtcNow().UtcDateTime;
        if (BookingExpiry.IsExpired(booking.Status, booking.CreatedAtUtc, booking.PaymentDeadlineUtc, now))
            return Result<CheckoutDto>.Fail(ErrorCodes.PaymentDeadlinePassed,
                "The payment deadline has passed. This booking will be cancelled automatically.");

        if (await _uow.Payments.GetByBookingAsync(booking.Id, ct) is not null)
            return Result<CheckoutDto>.Fail(ErrorCodes.InvalidStatusChange, "This booking is already paid.");

        return Result<CheckoutDto>.Ok(_mapper.Map<CheckoutDto>(booking));
    }
}
