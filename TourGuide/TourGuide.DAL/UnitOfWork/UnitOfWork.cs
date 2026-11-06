using Microsoft.EntityFrameworkCore.Storage;
using TourGuide.DAL.Data;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Repositories.Interfaces;

namespace TourGuide.DAL.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(
        AppDbContext context,
        IGuideProfileRepository guideProfiles,
        IGuideDocumentRepository guideDocuments,
        IBookingRepository bookings,
        IPaymentRepository payments,
        IReviewRepository reviews,
        IRepository<Language> languages,
        IRepository<Specialization> specializations)
    {
        _context = context;
        GuideProfiles = guideProfiles;
        GuideDocuments = guideDocuments;
        Bookings = bookings;
        Payments = payments;
        Reviews = reviews;
        Languages = languages;
        Specializations = specializations;
    }

    public IGuideProfileRepository GuideProfiles { get; }
    public IGuideDocumentRepository GuideDocuments { get; }
    public IBookingRepository Bookings { get; }
    public IPaymentRepository Payments { get; }
    public IReviewRepository Reviews { get; }
    public IRepository<Language> Languages { get; }
    public IRepository<Specialization> Specializations { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default) =>
        _context.Database.BeginTransactionAsync(ct);
}
