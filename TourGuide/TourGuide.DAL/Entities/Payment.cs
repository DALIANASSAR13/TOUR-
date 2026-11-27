namespace TourGuide.DAL.Entities;

// A simulated payment. Holds no card data (NFR-04).
public class Payment
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public DateTime PaidAtUtc { get; set; }

    public Booking Booking { get; set; } = null!;
}
