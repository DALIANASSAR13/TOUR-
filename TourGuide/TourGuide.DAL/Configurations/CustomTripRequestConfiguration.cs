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
    public class CustomTripRequestConfiguration : IEntityTypeConfiguration<CustomTripRequest>
    {
        public void Configure(EntityTypeBuilder<CustomTripRequest> builder)
        {
            builder.Property(c => c.Budget).HasPrecision(18, 2);
            builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);

            builder.HasOne(c => c.Guide)
                   .WithMany(g => g.CustomTripRequests)
                   .HasForeignKey(c => c.GuideId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
