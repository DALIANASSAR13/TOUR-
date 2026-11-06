using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Repositories.Interfaces;

// Specific repositories. Each later cycle adds its own query methods here
// (signatures in docs/class-diagram.md, section 6.1).

public interface IGuideProfileRepository : IRepository<GuideProfile> { }

public interface IGuideDocumentRepository : IRepository<GuideDocument> { }

public interface IBookingRepository : IRepository<Booking> { }

public interface IPaymentRepository : IRepository<Payment> { }

public interface IReviewRepository : IRepository<Review> { }
