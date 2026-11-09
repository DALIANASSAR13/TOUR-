using TourGuide.BLL.DTOs.Booking;

namespace TourGuide.BLL.Services.Interfaces;

// FR-11 – FR-13, FR-15, FR-16. Later cycles add PreviewPrice, Create, Accept, Reject and Withdraw.
public interface IBookingService
{
    Task<List<BookingDto>> GetForTouristAsync(string touristId, CancellationToken ct = default);
}
