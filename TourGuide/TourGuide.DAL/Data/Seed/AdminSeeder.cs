using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TourGuide.DAL.Constants;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Data.Seed;

// Creates the admin account at startup (admins are seeded, never signed up publicly).
// Reads Seed:AdminEmail, Seed:AdminFullName and Seed:AdminPassword from configuration.
// Keep the password out of Git: dotnet user-secrets set "Seed:AdminPassword" "<password>" --project TourGuide.PL
public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config, ILogger logger)
    {
        var email = config["Seed:AdminEmail"];
        var password = config["Seed:AdminPassword"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Admin seed skipped: set Seed:AdminEmail and Seed:AdminPassword (user secrets) to create the admin account.");
            return;
        }

        using var scope = services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await users.FindByEmailAsync(email) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = config["Seed:AdminFullName"] ?? "Administrator",
            CountryCode = config["Seed:AdminCountryCode"] ?? "EG",
            CreatedAtUtc = DateTime.UtcNow,
        };

        var created = await users.CreateAsync(admin, password);
        if (!created.Succeeded)
        {
            logger.LogError("Admin seed failed: {Errors}", string.Join("; ", created.Errors.Select(e => e.Description)));
            return;
        }

        await users.AddToRoleAsync(admin, Roles.Admin);
        logger.LogInformation("Admin account {Email} created.", email);
    }
}
