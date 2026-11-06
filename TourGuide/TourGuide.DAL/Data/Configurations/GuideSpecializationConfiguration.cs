using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Data.Configurations;

public class GuideSpecializationConfiguration : IEntityTypeConfiguration<GuideSpecialization>
{
    public void Configure(EntityTypeBuilder<GuideSpecialization> builder)
    {
        builder.HasKey(gs => new { gs.GuideProfileId, gs.SpecializationId });

        builder.HasOne(gs => gs.GuideProfile)
            .WithMany(g => g.Specializations)
            .HasForeignKey(gs => gs.GuideProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(gs => gs.Specialization)
            .WithMany(s => s.Guides)
            .HasForeignKey(gs => gs.SpecializationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
