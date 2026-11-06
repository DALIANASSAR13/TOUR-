using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings", t =>
        {
            // BookingType 0 = Hourly (2-10 hours), 1 = MultiDay (2-14 days). BR-04
            t.HasCheckConstraint("CK_Bookings_HoursOrDays",
                "([BookingType] = 0 AND [Hours] BETWEEN 2 AND 10 AND [Days] IS NULL) OR " +
                "([BookingType] = 1 AND [Days] BETWEEN 2 AND 14 AND [Hours] IS NULL)");
            t.HasCheckConstraint("CK_Bookings_EndAfterStart", "[EndUtc] > [StartUtc]");
            t.HasCheckConstraint("CK_Bookings_Prices", "[PricePerHourAtRequest] > 0 AND [TotalPrice] > 0");
        });

        builder.Property(b => b.PricePerHourAtRequest).HasPrecision(10, 2);
        builder.Property(b => b.TotalPrice).HasPrecision(10, 2);
        builder.Property(b => b.RowVersion).IsRowVersion();

        builder.HasIndex(b => new { b.GuideProfileId, b.Status, b.StartUtc });
        builder.HasIndex(b => b.TouristId);

        builder.HasOne(b => b.Tourist)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.TouristId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.GuideProfile)
            .WithMany(g => g.Bookings)
            .HasForeignKey(b => b.GuideProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
