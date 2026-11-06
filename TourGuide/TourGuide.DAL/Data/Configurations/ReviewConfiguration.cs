using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews", t =>
            t.HasCheckConstraint("CK_Reviews_Rating", "[Rating] >= 1 AND [Rating] <= 5"));

        builder.Property(r => r.Comment).HasMaxLength(1000);

        builder.HasIndex(r => r.BookingId).IsUnique();
        builder.HasIndex(r => r.GuideProfileId);

        builder.HasOne(r => r.Booking)
            .WithOne(b => b.Review)
            .HasForeignKey<Review>(r => r.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Tourist)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.TouristId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.GuideProfile)
            .WithMany(g => g.Reviews)
            .HasForeignKey(r => r.GuideProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
