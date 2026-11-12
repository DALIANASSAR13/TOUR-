using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.Payment;

namespace TourGuide.BLL.Services.Interfaces;

// FR-14.
public interface IPaymentService
{
    // What the owner tourist will pay. Only for an Accepted, unpaid booking before PaymentDeadlineUtc
    // (BR-07, BR-08). Saves nothing.
    Task<Result<CheckoutDto>> GetCheckoutAsync(string touristId, int bookingId, CancellationToken ct = default);
}
