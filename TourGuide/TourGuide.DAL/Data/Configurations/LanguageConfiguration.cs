using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourGuide.DAL.Data.Seed;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Data.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.Property(l => l.Name).HasMaxLength(50).IsRequired();
        builder.HasIndex(l => l.Name).IsUnique();
        builder.HasData(LookupSeed.Languages);
    }
}
