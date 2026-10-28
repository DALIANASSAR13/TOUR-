using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Data.Seeding;

namespace TourGuide.DAL.Data.Contexts
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Tourist> Tourists => Set<Tourist>();
        public DbSet<Guide> Guides => Set<Guide>();
        public DbSet<Tour> Tours => Set<Tour>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<CustomTripRequest> CustomTripRequests => Set<CustomTripRequest>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);  
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);


            modelBuilder.Entity<IdentityRole<int>>().HasData(SeedData.Roles);
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(SeedData.UserRoles);
        }
    }
}
