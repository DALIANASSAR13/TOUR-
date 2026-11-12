using TourGuide.DAL.Common;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Enums;

namespace TourGuide.DAL.Repositories.Interfaces;

// Specific repositories. Each later cycle adds its own query methods here
// (signatures in docs/class-diagram.md, section 6.1).
public interface IGuideProfileRepository : IRepository<GuideProfile>
{
    // Guide with user, languages and specializations (+ names) for BR-02 and the public profile (FR-09). Read-only.
    Task<GuideProfile?> GetWithDetailsAsync(int id, CancellationToken ct = default);

    // The logged-in guide's own profile (FR-16). Read-only.
    Task<GuideProfile?> GetByUserIdAsync(string userId, CancellationToken ct = default);

    // Guide profile with user, languages, and specializations tracked for editing.
    Task<GuideProfile?> GetByUserIdForUpdateAsync(string userId, CancellationToken ct = default);
}

public interface IGuideDocumentRepository : IRepository<GuideDocument>
{
    Task<GuideDocument?> GetByTypeAsync(int guideId, DocumentType type, CancellationToken ct = default);
}

public interface IBookingRepository : IRepository<Booking>
{
    // FR-15: the tourist's own bookings, newest first, with guide (+ user), payment and review. Read-only.
    Task<List<Booking>> GetForTouristAsync(string touristId, CancellationToken ct = default);

    // FR-16: requests and bookings made to this guide, newest first, with tourist, guide (time zone),
    // payment and review. Read-only.
    Task<List<Booking>> GetForGuideAsync(int guideId, CancellationToken ct = default);

    // BR-09: does [startUtc, endUtc) overlap one of the guide's Accepted or Confirmed bookings (other than excludeId)?
    Task<bool> HasOverlapAsync(int guideId, DateTime startUtc, DateTime endUtc, int? excludeId, CancellationToken ct = default);

    // FR-14 checkout: one booking with its guide (+ user). Read-only.
    Task<Booking?> GetWithGuideAsync(int id, CancellationToken ct = default);
}

public interface IPaymentRepository : IRepository<Payment>
{
    // FR-14: the booking's payment, or null when it is not paid (one payment per booking). Read-only.
    Task<Payment?> GetByBookingAsync(int bookingId, CancellationToken ct = default);
}

public interface IReviewRepository : IRepository<Review> { }
