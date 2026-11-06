using Microsoft.EntityFrameworkCore.Storage;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Repositories.Interfaces;

namespace TourGuide.DAL.UnitOfWork;

// The only place that saves. Services reach every repository through it.
public interface IUnitOfWork
{
    IGuideProfileRepository GuideProfiles { get; }
    IGuideDocumentRepository GuideDocuments { get; }
    IBookingRepository Bookings { get; }
    IPaymentRepository Payments { get; }
    IReviewRepository Reviews { get; }
    IRepository<Language> Languages { get; }
    IRepository<Specialization> Specializations { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
}
