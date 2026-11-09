using TourGuide.DAL.Common;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Queries;

namespace TourGuide.DAL.Repositories.Interfaces;

// Specific repositories. Each later cycle adds its own query methods here
// (signatures in docs/class-diagram.md, section 6.1).

public interface IGuideProfileRepository : IRepository<GuideProfile>
{
    // Guide with user, languages and specializations (+ names) for BR-02 and the public profile (FR-09). Read-only.
    Task<GuideProfile?> GetWithDetailsAsync(int id, CancellationToken ct = default);

    // The logged-in guide's own profile (FR-16). Read-only.
    Task<GuideProfile?> GetByUserIdAsync(string userId, CancellationToken ct = default);

    // FR-10: one page of searchable guides (always Approved + profile complete, BR-02), with user,
    // languages and specializations. Best rated first. Read-only.
    Task<PagedList<GuideProfile>> SearchAsync(GuideSearchQuery query, CancellationToken ct = default);

    // FR-10 country filter: the distinct country codes of searchable guides (BR-02). Read-only.
    Task<List<string>> GetSearchableCountryCodesAsync(CancellationToken ct = default);
}

public interface IGuideDocumentRepository : IRepository<GuideDocument> { }

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

public interface IReviewRepository : IRepository<Review>
{
    // FR-09: one page of a guide's reviews, newest first, with the tourist (name, country). Read-only.
    Task<PagedList<Review>> GetForGuideAsync(int guideId, int page, int pageSize, CancellationToken ct = default);
}
