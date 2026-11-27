using Microsoft.EntityFrameworkCore;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Repositories;

// Payment queries (the constructor is in SpecificRepositories.cs).
public partial class PaymentRepository
{
    public Task<Payment?> GetByBookingAsync(int bookingId, CancellationToken ct = default) =>
        Set.AsNoTracking().FirstOrDefaultAsync(p => p.BookingId == bookingId, ct);
}
