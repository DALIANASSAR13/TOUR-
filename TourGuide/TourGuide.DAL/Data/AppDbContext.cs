using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<GuideProfile> GuideProfiles => Set<GuideProfile>();
    public DbSet<GuideDocument> GuideDocuments => Set<GuideDocument>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<GuideLanguage> GuideLanguages => Set<GuideLanguage>();
    public DbSet<GuideSpecialization> GuideSpecializations => Set<GuideSpecialization>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
