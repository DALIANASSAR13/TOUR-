using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments", t =>
            t.HasCheckConstraint("CK_Payments_Amount", "[Amount] > 0"));

        builder.Property(p => p.Amount).HasPrecision(10, 2);
        builder.Property(p => p.TransactionReference).HasMaxLength(36).IsRequired();

        builder.HasIndex(p => p.BookingId).IsUnique();
        builder.HasIndex(p => p.TransactionReference).IsUnique();

        builder.HasOne(p => p.Booking)
            .WithOne(b => b.Payment)
            .HasForeignKey<Payment>(p => p.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
