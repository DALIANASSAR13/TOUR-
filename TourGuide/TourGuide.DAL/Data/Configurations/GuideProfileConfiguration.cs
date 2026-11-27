using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Data.Configurations;

public class GuideProfileConfiguration : IEntityTypeConfiguration<GuideProfile>
{
    public void Configure(EntityTypeBuilder<GuideProfile> builder)
    {
        builder.ToTable("GuideProfiles", t =>
        {
            t.HasCheckConstraint("CK_GuideProfiles_PricePerHour", "[PricePerHour] IS NULL OR ([PricePerHour] > 0 AND [PricePerHour] <= 1000)");
            t.HasCheckConstraint("CK_GuideProfiles_YearsOfExperience", "[YearsOfExperience] IS NULL OR ([YearsOfExperience] >= 0 AND [YearsOfExperience] <= 60)");
            t.HasCheckConstraint("CK_GuideProfiles_AverageRating", "[AverageRating] >= 0 AND [AverageRating] <= 5");
        });

        builder.Property(g => g.RejectionReason).HasMaxLength(500);
        builder.Property(g => g.CountryCode).HasMaxLength(2).IsFixedLength();
        builder.Property(g => g.City).HasMaxLength(100);
        builder.Property(g => g.TimeZoneId).HasMaxLength(64);
        builder.Property(g => g.Bio).HasMaxLength(1000);
        builder.Property(g => g.PhotoFileName).HasMaxLength(260);
        builder.Property(g => g.PricePerHour).HasPrecision(10, 2);
        builder.Property(g => g.AverageRating).HasPrecision(3, 2);

        builder.HasIndex(g => g.UserId).IsUnique();
        builder.HasIndex(g => new { g.VerificationStatus, g.CountryCode });

        builder.HasOne(g => g.User)
            .WithOne(u => u.GuideProfile)
            .HasForeignKey<GuideProfile>(g => g.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.ReviewedByAdmin)
            .WithMany()
            .HasForeignKey(g => g.ReviewedByAdminId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
