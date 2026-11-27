using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.Review;

namespace TourGuide.BLL.Services.Interfaces;

// FR-19, FR-20.
public interface IReviewService
{
    // Can this tourist review this booking right now? True only for their own Completed booking
    // that has no review yet (BR-13, BR-14, BR-15). Saves nothing.
    Task<bool> CanReviewAsync(string touristId, int bookingId, CancellationToken ct = default);

    // Saves the review and updates the guide's AverageRating and ReviewCount in one save (FR-20).
    Task<Result> AddAsync(string touristId, CreateReviewRequest request, CancellationToken ct = default);
}
