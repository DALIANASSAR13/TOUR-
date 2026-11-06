using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourGuide.DAL.Data.Seed;

namespace TourGuide.DAL.Data.Configurations;

// Seeds the 3 fixed roles: Tourist, Guide, Admin.
public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(RoleSeed.Roles);
    }
}
