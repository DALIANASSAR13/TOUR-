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
    public class GuideConfiguration : IEntityTypeConfiguration<Guide>
    {
        public void Configure(EntityTypeBuilder<Guide> builder)
        {
            builder.Property(g => g.Title).HasMaxLength(100);
            builder.Property(g => g.AboutMe).HasMaxLength(2000);
            builder.Property(g => g.PricePerHour).HasPrecision(18, 2);

            builder.HasData(SeedData.Guides);
        }
    }
}
