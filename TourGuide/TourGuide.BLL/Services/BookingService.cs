using AutoMapper;
using TourGuide.BLL.Domain;
using TourGuide.BLL.DTOs.Booking;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Enums;
using TourGuide.DAL.UnitOfWork;

namespace TourGuide.BLL.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public BookingService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
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
