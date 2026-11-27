using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Data.Configurations;

public class GuideLanguageConfiguration : IEntityTypeConfiguration<GuideLanguage>
{
    public void Configure(EntityTypeBuilder<GuideLanguage> builder)
    {
        builder.HasKey(gl => new { gl.GuideProfileId, gl.LanguageId });

        builder.HasOne(gl => gl.GuideProfile)
            .WithMany(g => g.Languages)
            .HasForeignKey(gl => gl.GuideProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(gl => gl.Language)
            .WithMany(l => l.Guides)
            .HasForeignKey(gl => gl.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
