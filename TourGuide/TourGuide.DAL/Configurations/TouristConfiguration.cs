using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourGuide.DAL.Data.Seeding;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Data.Configurations
{
    public class TouristConfiguration : IEntityTypeConfiguration<Tourist>
    {
        public void Configure(EntityTypeBuilder<Tourist> builder)
        {
            builder.HasData(SeedData.Tourists);
        }
    }
}