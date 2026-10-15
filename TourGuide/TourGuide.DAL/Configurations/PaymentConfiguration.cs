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
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(p => p.GuideFee).HasPrecision(18, 2);
            builder.Property(p => p.PlatformFee).HasPrecision(18, 2);
            builder.Property(p => p.Method).HasConversion<string>().HasMaxLength(20);
            builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);

            builder.HasOne(p => p.Booking)
                   .WithOne(b => b.Payment)
                   .HasForeignKey<Payment>(p => p.BookingId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
