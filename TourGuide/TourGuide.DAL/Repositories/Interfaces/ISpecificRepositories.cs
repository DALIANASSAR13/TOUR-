using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Repositories.Interfaces;

// Specific repositories. Each later cycle adds its own query methods here
// (signatures in docs/class-diagram.md, section 6.1).

public interface IGuideProfileRepository : IRepository<GuideProfile>
{
    // Guide with user, languages and specializations (BR-02 completeness check). Read-only.
    Task<GuideProfile?> GetWithDetailsAsync(int id, CancellationToken ct = default);
}

public interface IGuideDocumentRepository : IRepository<GuideDocument> { }

public interface IBookingRepository : IRepository<Booking>
{
    // FR-15: the tourist's own bookings, newest first, with guide (+ user), payment and review. Read-only.
    Task<List<Booking>> GetForTouristAsync(string touristId, CancellationToken ct = default);
}

public interface IPaymentRepository : IRepository<Payment> { }

public interface IReviewRepository : IRepository<Review> { }
