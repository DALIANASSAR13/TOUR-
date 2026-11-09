using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.Booking;

namespace TourGuide.BLL.Services.Interfaces;

// FR-11 – FR-13, FR-15, FR-16. Later cycles add Create, Accept, Reject and Withdraw.
public interface IBookingService
{
    // FR-12: price before the request is sent. Saves nothing.
    Task<Result<PriceQuoteDto>> PreviewPriceAsync(CreateBookingRequest request, CancellationToken ct = default);

    Task<List<BookingDto>> GetForTouristAsync(string touristId, CancellationToken ct = default);
}
