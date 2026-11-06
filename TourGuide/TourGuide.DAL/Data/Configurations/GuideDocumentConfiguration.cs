using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Data.Configurations;

public class GuideDocumentConfiguration : IEntityTypeConfiguration<GuideDocument>
{
    public void Configure(EntityTypeBuilder<GuideDocument> builder)
    {
        builder.ToTable("GuideDocuments", t =>
            t.HasCheckConstraint("CK_GuideDocuments_SizeBytes", "[SizeBytes] > 0 AND [SizeBytes] <= 5242880"));

        builder.Property(d => d.StoredFileName).HasMaxLength(260).IsRequired();
        builder.Property(d => d.OriginalFileName).HasMaxLength(260).IsRequired();
        builder.Property(d => d.ContentType).HasMaxLength(100).IsRequired();

        builder.HasIndex(d => new { d.GuideProfileId, d.DocumentType }).IsUnique();

        builder.HasOne(d => d.GuideProfile)
            .WithMany(g => g.Documents)
            .HasForeignKey(d => d.GuideProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
