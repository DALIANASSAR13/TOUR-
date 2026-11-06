using Microsoft.AspNetCore.Identity;
using TourGuide.DAL.Constants;

namespace TourGuide.DAL.Data.Seed;

// Fixed ids and stamps, so migrations stay the same on every machine.
public static class RoleSeed
{
    public static readonly IdentityRole[] Roles =
    {
        new() { Id = "role-tourist", Name = Constants.Roles.Tourist, NormalizedName = "TOURIST", ConcurrencyStamp = "role-tourist-stamp" },
        new() { Id = "role-guide",   Name = Constants.Roles.Guide,   NormalizedName = "GUIDE",   ConcurrencyStamp = "role-guide-stamp" },
        new() { Id = "role-admin",   Name = Constants.Roles.Admin,   NormalizedName = "ADMIN",   ConcurrencyStamp = "role-admin-stamp" },
    };
}
