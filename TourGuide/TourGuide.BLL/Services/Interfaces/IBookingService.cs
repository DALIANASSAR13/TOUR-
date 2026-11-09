using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.Booking;

namespace TourGuide.BLL.Services.Interfaces;

// FR-11 – FR-13, FR-15, FR-16. Later cycles add Reject and Withdraw.
public interface IBookingService
{
    // FR-12: price before the request is sent. Saves nothing.
    Task<Result<PriceQuoteDto>> PreviewPriceAsync(CreateBookingRequest request, CancellationToken ct = default);

    // FR-11: save a Pending request with the price copied now (BR-11).
    Task<Result<BookingDto>> CreateAsync(string touristId, CreateBookingRequest request, CancellationToken ct = default);

    Task<List<BookingDto>> GetForTouristAsync(string touristId, CancellationToken ct = default);

    // FR-16: requests and bookings made to the logged-in guide. Fails when the guide is not Approved (BR-01).
    Task<Result<List<BookingDto>>> GetForGuideAsync(string guideUserId, CancellationToken ct = default);

    // FR-13: the owner guide accepts a Pending request (BR-06, BR-08, BR-09).
    Task<Result> AcceptAsync(string guideUserId, int bookingId, CancellationToken ct = default);
}
