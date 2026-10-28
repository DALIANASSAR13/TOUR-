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

    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Property(b => b.TotalPrice).HasPrecision(18, 2);
            builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);

            builder.HasOne(b => b.Tour)
                   .WithMany(t => t.Bookings)
                   .HasForeignKey(b => b.TourId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Tourist)
                   .WithMany(t => t.TripHistory)
                   .HasForeignKey(b => b.TouristId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(SeedData.Bookings);
        }
    }
}
