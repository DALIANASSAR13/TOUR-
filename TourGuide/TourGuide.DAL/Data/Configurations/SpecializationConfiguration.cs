using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourGuide.DAL.Data.Seed;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Data.Configurations;

public class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
{
    public void Configure(EntityTypeBuilder<Specialization> builder)
    {
        builder.Property(s => s.Name).HasMaxLength(50).IsRequired();
        builder.HasIndex(s => s.Name).IsUnique();
        builder.HasData(LookupSeed.Specializations);
    }
}
