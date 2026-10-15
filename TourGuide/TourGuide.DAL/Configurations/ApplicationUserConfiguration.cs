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

    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Country).IsRequired().HasMaxLength(60);
            builder.Property(u => u.ProfileImageUrl).HasMaxLength(500);
            builder.Property(u => u.GoogleId).HasMaxLength(100);
            builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);

      
            builder.HasDiscriminator<string>("UserType")
                   .HasValue<ApplicationUser>("User")
                   .HasValue<Tourist>("Tourist")
                   .HasValue<Guide>("Guide");
        }
    }
}
