using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.DAL.Data.Seeding;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Configurations
{
    public class TourConfiguration : IEntityTypeConfiguration<Tour>
    {
        public void Configure(EntityTypeBuilder<Tour> builder)
        {
            builder.Property(t => t.Title).IsRequired().HasMaxLength(150);
            builder.Property(t => t.Description).IsRequired().HasMaxLength(4000);
            builder.Property(t => t.ImageUrl).HasMaxLength(500);
            builder.Property(t => t.PricePerPerson).HasPrecision(18, 2);
            builder.Property(t => t.Type).HasConversion<string>().HasMaxLength(30);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Tour_SlotsLeft", "[SlotsLeft] >= 0");
                t.HasCheckConstraint("CK_Tour_MaxGroupSize", "[MaxGroupSize] > 0");
            });

            builder.HasOne(t => t.Guide)
                   .WithMany(g => g.Tours)
                   .HasForeignKey(t => t.GuideId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(SeedData.Tours);
        }
    }
}
