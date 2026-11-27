using Microsoft.EntityFrameworkCore;
using TourGuide.DAL.Data;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Enums;
using TourGuide.DAL.Repositories.Interfaces;

namespace TourGuide.DAL.Repositories;

// Specific repositories. `partial` so each later cycle can add its queries in its own file.
public partial class GuideProfileRepository : Repository<GuideProfile>, IGuideProfileRepository
{
    public GuideProfileRepository(AppDbContext context) : base(context) { }
}
public partial class GuideDocumentRepository : Repository<GuideDocument>, IGuideDocumentRepository
{
    private readonly AppDbContext _context;

    public GuideDocumentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<GuideDocument?> GetByTypeAsync(int guideId, DocumentType type, CancellationToken ct = default) =>
        _context.Set<GuideDocument>().FirstOrDefaultAsync(d => d.GuideProfileId == guideId && d.DocumentType == type, ct);

    public async Task<IReadOnlyList<GuideDocument>> GetByGuideAsync(int guideId, CancellationToken ct = default) =>
        await _context.Set<GuideDocument>()
            .AsNoTracking()
            .Where(d => d.GuideProfileId == guideId)
            .OrderBy(d => d.DocumentType)
            .ToListAsync(ct);
}

public partial class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(AppDbContext context) : base(context) { }
}

public partial class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context) { }
}

public partial class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(AppDbContext context) : base(context) { }
}