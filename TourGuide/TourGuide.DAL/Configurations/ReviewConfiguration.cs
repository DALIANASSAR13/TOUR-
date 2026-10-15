using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.Property(r => r.Comment).HasMaxLength(1000);

            builder.ToTable(t =>
                t.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5"));

            builder.HasIndex(r => new { r.TourId, r.TouristId }).IsUnique();

            builder.HasOne(r => r.Tour)
                   .WithMany(t => t.Reviews)
                   .HasForeignKey(r => r.TourId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Tourist)
                   .WithMany(t => t.Reviews)
                   .HasForeignKey(r => r.TouristId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

