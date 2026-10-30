using Microsoft.AspNetCore.Identity;
using System;
using TourGuide.DAL.Data.Enums;
using TourGuide.DAL.Entities;

namespace TourGuide.DAL.Data.Seeding
{
    /// <summary>
    /// Static seed data used by each entity configuration via builder.HasData(...).
    /// Rules: every value is fixed (no DateTime.Now / Guid.NewGuid) and every Id is explicit.
    /// ID map:  Guides 1-4 | Tourists 5-8 (they share AspNetUsers) | Roles: 1 = Guide, 2 = Tourist
    /// </summary>
    public static class SeedData
    {
        // Generate ONCE with: new PasswordHasher<ApplicationUser>().HashPassword(null!, "Test@1234")
        // then paste the output here. All seeded users share this password.
        private const string DefaultPasswordHash = "PASTE_GENERATED_HASH_HERE";

        private static readonly DateTime Created = new(2026, 1, 15, 10, 0, 0);

        // ---------------------------------------------------------------- Roles
        public static readonly IdentityRole<int>[] Roles =
        {
            new() { Id = 1, Name = "Guide",   NormalizedName = "GUIDE",   ConcurrencyStamp = "role-guide-0001" },
            new() { Id = 2, Name = "Tourist", NormalizedName = "TOURIST", ConcurrencyStamp = "role-tourist-0002" },
        };

        public static readonly IdentityUserRole<int>[] UserRoles =
        {
            new() { UserId = 1, RoleId = 1 },
            new() { UserId = 2, RoleId = 1 },
            new() { UserId = 3, RoleId = 1 },
            new() { UserId = 4, RoleId = 1 },
            new() { UserId = 5, RoleId = 2 },
            new() { UserId = 6, RoleId = 2 },
            new() { UserId = 7, RoleId = 2 },
            new() { UserId = 8, RoleId = 2 },
        };

        // ---------------------------------------------------------------- Guides
        // Do NOT set UserType: EF fills the TPH discriminator for Guide/Tourist automatically.
        public static readonly Guide[] Guides =
        {
            new()
            {
                Id = 1, FullName = "Ahmed Hassan", Role = UserRole.Guide, Country = "Egypt", CreatedAt = Created,
                UserName = "ahmed.hassan@tourguide.com", NormalizedUserName = "AHMED.HASSAN@TOURGUIDE.COM",
                Email = "ahmed.hassan@tourguide.com", NormalizedEmail = "AHMED.HASSAN@TOURGUIDE.COM",
                EmailConfirmed = true, PasswordHash = DefaultPasswordHash,
                SecurityStamp = "stamp-user-0001", ConcurrencyStamp = "concurrency-user-0001",
                LockoutEnabled = true,
                ProfileImageUrl = "/images/guides/ahmed.jpg",
                Title = "Egyptologist & Cairo/Giza Guide",
                AboutMe = "Licensed Egyptologist with over 10 years of experience guiding visitors through the Giza Plateau, Saqqara and Old Cairo.",
                ToursCompleted = 120, IsVerified = true, PricePerHour = 25.00m,AccessFailedCount = 0
            },
            new()
            {
                Id = 2, FullName = "Mariam Adel", Role = UserRole.Guide, Country = "Egypt", CreatedAt = Created,
                UserName = "mariam.adel@tourguide.com", NormalizedUserName = "MARIAM.ADEL@TOURGUIDE.COM",
                Email = "mariam.adel@tourguide.com", NormalizedEmail = "MARIAM.ADEL@TOURGUIDE.COM",
                EmailConfirmed = true, PasswordHash = DefaultPasswordHash,
                SecurityStamp = "stamp-user-0002", ConcurrencyStamp = "concurrency-user-0002",
                LockoutEnabled = true,
                ProfileImageUrl = "/images/guides/mariam.jpg",
                Title = "Luxor & West Bank Specialist",
                AboutMe = "Born and raised in Luxor. I love sharing the stories behind Karnak, Luxor Temple and the Valley of the Kings.",
                ToursCompleted = 85, IsVerified = true, PricePerHour = 22.00m, AccessFailedCount = 0
            },
            new()
            {
                Id = 3, FullName = "Omar Khaled", Role = UserRole.Guide, Country = "Egypt", CreatedAt = Created,
                UserName = "omar.khaled@tourguide.com", NormalizedUserName = "OMAR.KHALED@TOURGUIDE.COM",
                Email = "omar.khaled@tourguide.com", NormalizedEmail = "OMAR.KHALED@TOURGUIDE.COM",
                EmailConfirmed = true, PasswordHash = DefaultPasswordHash,
                SecurityStamp = "stamp-user-0003", ConcurrencyStamp = "concurrency-user-0003",
                LockoutEnabled = true,
                ProfileImageUrl = "/images/guides/omar.jpg",
                Title = "Aswan & Nubian Culture Expert",
                AboutMe = "Nubian guide who knows every corner of Aswan, from Philae Temple to the colorful villages along the Nile.",
                ToursCompleted = 60, IsVerified = true, PricePerHour = 20.00m,AccessFailedCount = 0
            },
            new()
            {
                Id = 4, FullName = "Nour Salah", Role = UserRole.Guide, Country = "Egypt", CreatedAt = Created,
                UserName = "nour.salah@tourguide.com", NormalizedUserName = "NOUR.SALAH@TOURGUIDE.COM",
                Email = "nour.salah@tourguide.com", NormalizedEmail = "NOUR.SALAH@TOURGUIDE.COM",
                EmailConfirmed = true, PasswordHash = DefaultPasswordHash,
                SecurityStamp = "stamp-user-0004", ConcurrencyStamp = "concurrency-user-0004",
                LockoutEnabled = true,
                ProfileImageUrl = "/images/guides/nour.jpg",
                Title = "Alexandria Heritage Guide",
                AboutMe = "History graduate who loves Alexandria's Greco-Roman past, seaside cafes and hidden local spots.",
                ToursCompleted = 40, IsVerified = false, PricePerHour = 18.00m,AccessFailedCount = 0
            },
        };

        // ---------------------------------------------------------------- Tourists
        public static readonly Tourist[] Tourists =
        {
            new()
            {
                Id = 5, FullName = "Emily Carter", Role = UserRole.Tourist, Country = "United Kingdom", CreatedAt = Created,
                UserName = "emily.carter@example.com", NormalizedUserName = "EMILY.CARTER@EXAMPLE.COM",
                Email = "emily.carter@example.com", NormalizedEmail = "EMILY.CARTER@EXAMPLE.COM",
                EmailConfirmed = true, PasswordHash = DefaultPasswordHash,
                SecurityStamp = "stamp-user-0005", ConcurrencyStamp = "concurrency-user-0005",
                LockoutEnabled = true,AccessFailedCount = 0
            },
            new()
            {
                Id = 6, FullName = "Lucas Meyer", Role = UserRole.Tourist, Country = "Germany", CreatedAt = Created,
                UserName = "lucas.meyer@example.com", NormalizedUserName = "LUCAS.MEYER@EXAMPLE.COM",
                Email = "lucas.meyer@example.com", NormalizedEmail = "LUCAS.MEYER@EXAMPLE.COM",
                EmailConfirmed = true, PasswordHash = DefaultPasswordHash,
                SecurityStamp = "stamp-user-0006", ConcurrencyStamp = "concurrency-user-0006",
                LockoutEnabled = true, AccessFailedCount = 0
            },
            new()
            {
                Id = 7, FullName = "Sofia Rossi", Role = UserRole.Tourist, Country = "Italy", CreatedAt = Created,
                UserName = "sofia.rossi@example.com", NormalizedUserName = "SOFIA.ROSSI@EXAMPLE.COM",
                Email = "sofia.rossi@example.com", NormalizedEmail = "SOFIA.ROSSI@EXAMPLE.COM",
                EmailConfirmed = true, PasswordHash = DefaultPasswordHash,
                SecurityStamp = "stamp-user-0007", ConcurrencyStamp = "concurrency-user-0007",
                LockoutEnabled = true,AccessFailedCount = 0
            },
            new()
            {
                Id = 8, FullName = "Daniel Kim", Role = UserRole.Tourist, Country = "South Korea", CreatedAt = Created,
                UserName = "daniel.kim@example.com", NormalizedUserName = "DANIEL.KIM@EXAMPLE.COM",
                Email = "daniel.kim@example.com", NormalizedEmail = "DANIEL.KIM@EXAMPLE.COM",
                EmailConfirmed = true, PasswordHash = DefaultPasswordHash,
                SecurityStamp = "stamp-user-0008", ConcurrencyStamp = "concurrency-user-0008",
                LockoutEnabled = true,AccessFailedCount = 0
            },
        };

        // ---------------------------------------------------------------- Tours
        // Tours 1-4 are upcoming and active. Tours 5-6 already took place (inactive) and carry the reviews.
        // SlotsLeft = MaxGroupSize - sum(NumberOfPeople of its bookings), so the CHECK constraints always hold.
        public static readonly Tour[] Tours =
        {
            new()
            {
                Id = 1, Title = "Pyramids of Giza & Sphinx Day Tour",
                Location = "Red Sea, Hurghada",
                Description = "Walk the Giza Plateau with an Egyptologist: the Great Pyramid, the Sphinx, the Valley Temple and the best panoramic viewpoint.",
                DurationDays = 1, PricePerPerson = 60.00m, MaxGroupSize = 12, SlotsLeft = 7,
                Type = TourType.Historical, AverageRating = 0, IsActive = true, ImageUrl = "/images/tours/pyramids.jpg",
                StartDate = new DateTime(2026, 12, 12, 8, 0, 0), EndDate = new DateTime(2026, 12, 12, 16, 0, 0), GuideId = 1
            },
            new()
            {
                Id = 2, Title = "Luxor East & West Bank Highlights",
                Location = "Luxor & Aswan, Egypt",
                Description = "Two days across Luxor: Karnak and Luxor Temples on the East Bank, then the Valley of the Kings and Hatshepsut Temple on the West Bank.",
                DurationDays = 2, PricePerPerson = 140.00m, MaxGroupSize = 10, SlotsLeft = 6,
                Type = TourType.Historical, AverageRating = 0, IsActive = true, ImageUrl = "/images/tours/luxor.jpg",
                StartDate = new DateTime(2027, 1, 15, 8, 0, 0), EndDate = new DateTime(2027, 1, 16, 17, 0, 0), GuideId = 2
            },
            new()
            {
                Id = 3, Title = "Aswan, Philae & Nubian Village Experience",
                Location = "Giza, Egypt",
                Description = "Felucca ride on the Nile, Philae Temple, the High Dam and a home-cooked lunch in a Nubian village.",
                DurationDays = 3, PricePerPerson = 210.00m, MaxGroupSize = 8, SlotsLeft = 8,
                Type = TourType.Cruise, AverageRating = 0, IsActive = true, ImageUrl = "/images/tours/aswan.jpg",
                StartDate = new DateTime(2027, 2, 5, 9, 0, 0), EndDate = new DateTime(2027, 2, 7, 18, 0, 0), GuideId = 3
            },
            new()
            {
                Id = 4, Title = "Alexandria Coastal Heritage Walk",
                Location = "Alexandria, Egypt",
                Description = "Stroll the Corniche, visit the Bibliotheca Alexandrina, Qaitbay Citadel and the Roman Amphitheatre, finishing with fresh seafood.",
                DurationDays = 1, PricePerPerson = 45.00m, MaxGroupSize = 15, SlotsLeft = 15,
                Type = TourType.Walking, AverageRating = 0, IsActive = true, ImageUrl = "/images/tours/alexandria.jpg",
                StartDate = new DateTime(2027, 3, 6, 9, 0, 0), EndDate = new DateTime(2027, 3, 6, 17, 0, 0), GuideId = 4
            },
            new()
            {
                Id = 5, Title = "Old Cairo & Khan el-Khalili Evening",
                Description = "Coptic Cairo, Al-Muizz Street and the bazaar after sunset, with street-food tastings along the way.",
                DurationDays = 1, PricePerPerson = 40.00m, MaxGroupSize = 10, SlotsLeft = 6,
                Type = TourType.Walking, AverageRating = 4.5, IsActive = false, ImageUrl = "/images/tours/old-cairo.jpg",
                StartDate = new DateTime(2026, 9, 12, 16, 0, 0), EndDate = new DateTime(2026, 9, 12, 22, 0, 0), GuideId = 1
            },
            new()
            {
                Id = 6, Title = "Luxor Sunrise Hot Air Balloon",
                Description = "Float over the Nile and the temples of Luxor at sunrise, followed by breakfast on the West Bank.",
                DurationDays = 1, PricePerPerson = 95.00m, MaxGroupSize = 6, SlotsLeft = 4,
                Type = TourType.Photography, AverageRating = 5.0, IsActive = false, ImageUrl = "/images/tours/balloon.jpg",
                StartDate = new DateTime(2026, 10, 2, 5, 0, 0), EndDate = new DateTime(2026, 10, 2, 10, 0, 0), GuideId = 2
            },
        };

        // ---------------------------------------------------------------- Bookings
        public static readonly Booking[] Bookings =
        {
            new() { Id = 1, NumberOfPeople = 2, Status = BookingStatus.Confirmed, TotalPrice = 120.00m, CreatedAt = new DateTime(2026, 10, 20, 12, 0, 0), TourId = 1, TouristId = 5 },
            new() { Id = 2, NumberOfPeople = 3, Status = BookingStatus.Pending,   TotalPrice = 180.00m, CreatedAt = new DateTime(2026, 10, 25, 15, 30, 0), TourId = 1, TouristId = 8 },
            new() { Id = 3, NumberOfPeople = 4, Status = BookingStatus.Confirmed, TotalPrice = 560.00m, CreatedAt = new DateTime(2026, 10, 22, 9, 15, 0), TourId = 2, TouristId = 6 },
            new() { Id = 4, NumberOfPeople = 2, Status = BookingStatus.Completed, TotalPrice = 80.00m,  CreatedAt = new DateTime(2026, 8, 30, 11, 0, 0),  TourId = 5, TouristId = 5 },
            new() { Id = 5, NumberOfPeople = 2, Status = BookingStatus.Completed, TotalPrice = 80.00m,  CreatedAt = new DateTime(2026, 9, 1, 14, 0, 0),   TourId = 5, TouristId = 6 },
            new() { Id = 6, NumberOfPeople = 2, Status = BookingStatus.Completed, TotalPrice = 190.00m, CreatedAt = new DateTime(2026, 9, 10, 10, 0, 0),  TourId = 6, TouristId = 7 },
        };

        // ---------------------------------------------------------------- Payments (one per booking; 90% guide / 10% platform)
        // Upcoming confirmed bookings: money is HeldInEscrow. Completed tours: Paid. Pending booking: Pending.
        public static readonly Payment[] Payments =
        {
            new() { Id = 1, Method = PaymentMethod.CreditCard, Status = PaymentStatus.HeldInEscrow, GuideFee = 108.00m, PlatformFee = 12.00m, PaidAt = new DateTime(2026, 10, 20, 12, 5, 0), BookingId = 1 },
            new() { Id = 2, Method = PaymentMethod.PayPal,     Status = PaymentStatus.Pending,      GuideFee = 162.00m, PlatformFee = 18.00m, PaidAt = null,                                BookingId = 2 },
            new() { Id = 3, Method = PaymentMethod.ApplePay,   Status = PaymentStatus.HeldInEscrow, GuideFee = 504.00m, PlatformFee = 56.00m, PaidAt = new DateTime(2026, 10, 22, 9, 20, 0), BookingId = 3 },
            new() { Id = 4, Method = PaymentMethod.CreditCard, Status = PaymentStatus.Paid,         GuideFee = 72.00m,  PlatformFee = 8.00m,  PaidAt = new DateTime(2026, 8, 30, 11, 5, 0),  BookingId = 4 },
            new() { Id = 5, Method = PaymentMethod.PayPal,     Status = PaymentStatus.Paid,         GuideFee = 72.00m,  PlatformFee = 8.00m,  PaidAt = new DateTime(2026, 9, 1, 14, 5, 0),   BookingId = 5 },
            new() { Id = 6, Method = PaymentMethod.CreditCard, Status = PaymentStatus.Paid,         GuideFee = 171.00m, PlatformFee = 19.00m, PaidAt = new DateTime(2026, 9, 10, 10, 5, 0),  BookingId = 6 },
        };

        // ---------------------------------------------------------------- Reviews (Rating must be 1-5; unique per Tour + Tourist)
        public static readonly Review[] Reviews =
        {
            new() { Id = 1, Rating = 5, Comment = "Ahmed made Old Cairo come alive. The food stops were the highlight of our trip!", CreatedAt = new DateTime(2026, 9, 14, 10, 0, 0), TourId = 5, TouristId = 5 },
            new() { Id = 2, Rating = 4, Comment = "Great stories and a well-paced evening. It got a little crowded in the bazaar.",      CreatedAt = new DateTime(2026, 9, 15, 18, 0, 0), TourId = 5, TouristId = 6 },
            new() { Id = 3, Rating = 5, Comment = "Unforgettable sunrise over Luxor. Mariam organized everything perfectly.",           CreatedAt = new DateTime(2026, 10, 3, 9, 0, 0),  TourId = 6, TouristId = 7 },
        };

        // ---------------------------------------------------------------- Custom trip requests
        public static readonly CustomTripRequest[] CustomTripRequests =
        {
            new() { Id = 1, StartDate = new DateTime(2027, 3, 1),  EndDate = new DateTime(2027, 3, 7),  Budget = 1500.00m, Status = CustomTripStatus.Requested,  CreatedAt = new DateTime(2026, 10, 26, 13, 0, 0), TouristId = 7, GuideId = 1 },
            new() { Id = 2, StartDate = new DateTime(2027, 4, 10), EndDate = new DateTime(2027, 4, 15), Budget = 1200.00m, Status = CustomTripStatus.InProgress, CreatedAt = new DateTime(2026, 10, 18, 16, 0, 0), TouristId = 8, GuideId = 3 },
        };

        // ---------------------------------------------------------------- Notifications
        // NotificationType only has NewBookingRequest, so every notification goes to the guide who received the booking.
        public static readonly Notification[] Notifications =
        {
            new() { Id = 1, UserId = 1, Message = "New booking request for Pyramids of Giza & Sphinx Day Tour (2 people).", NotificationType = NotificationType.NewBookingRequest, IsRead = true,  CreatedAt = new DateTime(2026, 10, 20, 12, 1, 0) },
            new() { Id = 2, UserId = 1, Message = "New booking request for Pyramids of Giza & Sphinx Day Tour (3 people).", NotificationType = NotificationType.NewBookingRequest, IsRead = false, CreatedAt = new DateTime(2026, 10, 25, 15, 31, 0) },
            new() { Id = 3, UserId = 2, Message = "New booking request for Luxor East & West Bank Highlights (4 people).",     NotificationType = NotificationType.NewBookingRequest, IsRead = false, CreatedAt = new DateTime(2026, 10, 22, 9, 16, 0) },
            new() { Id = 4, UserId = 2, Message = "New booking request for Luxor Sunrise Hot Air Balloon (2 people).",      NotificationType = NotificationType.NewBookingRequest, IsRead = true,  CreatedAt = new DateTime(2026, 9, 10, 10, 1, 0) },
        };
    }
}
