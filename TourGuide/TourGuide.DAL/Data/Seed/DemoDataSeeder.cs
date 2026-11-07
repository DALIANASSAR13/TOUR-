using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TourGuide.DAL.Constants;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Enums;

namespace TourGuide.DAL.Data.Seed;

// Rules and paths the demo seed needs. Program.cs fills them from BookingRules and FileStorage,
// so no limit is typed twice (AGENTS.md -> DRY).
public sealed record DemoSeedSettings(
    string Password,
    string UploadsRootPath,
    int HoursPerDay,
    int PendingExpiryHours,
    int PaymentWindowHours);

// Development-only demo data for every table: users + roles, guide profiles (all 3 statuses),
// documents (with real placeholder files), languages, specializations, bookings in all 6 statuses,
// payments and reviews. Runs once: it does nothing if any guide profile already exists.
// Dates are relative to the moment it runs, so Pending/Accepted rows are still "live".
public static class DemoDataSeeder
{
    private const string Domain = "tourguide.local";

    public static async Task SeedAsync(IServiceProvider services, DemoSeedSettings settings, ILogger logger)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await db.Set<GuideProfile>().AnyAsync())
        {
            logger.LogInformation("Demo seed skipped: guide profiles already exist.");
            return;
        }

        var now = DateTime.UtcNow;
        await using var tx = await db.Database.BeginTransactionAsync();

        // ---------- Users ----------
        var admin = (await users.GetUsersInRoleAsync(Roles.Admin)).FirstOrDefault()
                    ?? await CreateUserAsync(users, "admin", "Demo Admin", "EG", Roles.Admin, settings, now.AddDays(-60));

        var emma = await CreateUserAsync(users, "emma.schmidt", "Emma Schmidt", "DE", Roles.Tourist, settings, now.AddDays(-45));
        var john = await CreateUserAsync(users, "john.carter", "John Carter", "US", Roles.Tourist, settings, now.AddDays(-40));
        var sophie = await CreateUserAsync(users, "sophie.martin", "Sophie Martin", "FR", Roles.Tourist, settings, now.AddDays(-35));

        // ---------- Guides ----------
        var omar = await CreateApprovedGuideAsync(users, db, settings, admin, now, "omar.hassan", "Omar Hassan", "EG", "Giza", "Africa/Cairo", 25m, 8,
            "Egyptologist who has walked the Giza plateau for eight years. Pyramids, Sphinx and the stories behind them.", new[] { 1, 2, 3 }, new[] { 1, 4 });
        var yasmin = await CreateApprovedGuideAsync(users, db, settings, admin, now, "yasmin.elmasry", "Yasmin El-Masry", "EG", "Luxor", "Africa/Cairo", 22m, 6,
            "Luxor born and raised. Karnak, the Valley of the Kings and quiet Nile sunsets.", new[] { 1, 2, 4 }, new[] { 1, 3 });
        var karim = await CreateApprovedGuideAsync(users, db, settings, admin, now, "karim.adel", "Karim Adel", "EG", "Cairo", "Africa/Cairo", 30m, 10,
            "Islamic and Coptic Cairo, old markets and street photography walks.", new[] { 1, 2, 5 }, new[] { 1, 5 });
        var mona = await CreateApprovedGuideAsync(users, db, settings, admin, now, "mona.fawzy", "Mona Fawzy", "EG", "Aswan", "Africa/Cairo", 20m, 5,
            "Nubian villages, Philae temple and felucca trips on the Nile.", new[] { 1, 2, 6 }, new[] { 3, 4 });
        var kenji = await CreateApprovedGuideAsync(users, db, settings, admin, now, "kenji.sato", "Kenji Sato", "JP", "Kyoto", "Asia/Tokyo", 35m, 12,
            "Temples, gardens and back streets of Kyoto, at a slow pace.", new[] { 1, 9 }, new[] { 1, 4, 5 });
        var hany = await CreateApprovedGuideAsync(users, db, settings, admin, now, "hany.samir", "Hany Samir", "EG", "Alexandria", "Africa/Cairo", 18m, 4,
            "Alexandria's Roman ruins, the Library and the Corniche on foot.", new[] { 1, 2 }, new[] { 4, 5 });
        var nour = await CreateApprovedGuideAsync(users, db, settings, admin, now, "nour.ibrahim", "Nour Ibrahim", "EG", "Luxor", "Africa/Cairo", 16m, 3,
            "West Bank tombs and desert camping under the stars.", new[] { 1, 2, 3 }, new[] { 1, 2 });

        // Approved but profile not complete -> hidden from search (BR-02).
        var mahmoudUser = await CreateUserAsync(users, "mahmoud.ali", "Mahmoud Ali", "EG", Roles.Guide, settings, now.AddDays(-10));
        var mahmoud = AddGuideProfile(db, settings, mahmoudUser, now.AddDays(-10));
        mahmoud.VerificationStatus = VerificationStatus.Approved;
        mahmoud.ReviewedAtUtc = now.AddDays(-9);
        mahmoud.ReviewedByAdminId = admin.Id;

        // Waiting for the admin (FR-04).
        var ahmedUser = await CreateUserAsync(users, "ahmed.tarek", "Ahmed Tarek", "EG", Roles.Guide, settings, now.AddDays(-1));
        AddGuideProfile(db, settings, ahmedUser, now.AddDays(-1));

        // Rejected with a reason (FR-06) -> can re-apply (FR-07).
        var lailaUser = await CreateUserAsync(users, "laila.mostafa", "Laila Mostafa", "EG", Roles.Guide, settings, now.AddDays(-5));
        var laila = AddGuideProfile(db, settings, lailaUser, now.AddDays(-5));
        laila.VerificationStatus = VerificationStatus.Rejected;
        laila.RejectionReason = "The criminal record certificate is older than 6 months. Please upload a recent one.";
        laila.ReviewedAtUtc = now.AddDays(-4);
        laila.ReviewedByAdminId = admin.Id;

        await db.SaveChangesAsync();

        // ---------- Bookings ----------
        var b = new BookingFactory(settings, now);
        var bookings = new List<Booking>();

        // Emma: one booking per status, matching UI screen 6.
        bookings.Add(b.Accepted(b.Hourly(emma, omar, 6, 10, 6, now.AddHours(-6)), respondedAt: now.AddHours(-2)));
        bookings.Add(b.Pending(b.MultiDay(emma, yasmin, 13, 3, now.AddHours(-6))));
        bookings.Add(b.Confirmed(b.Accepted(b.Hourly(emma, karim, 4, 9, 4, now.AddDays(-8)), now.AddDays(-7)), now.AddDays(-7).AddHours(3)));
        bookings.Add(b.Completed(b.Confirmed(b.Accepted(b.Hourly(emma, mona, -5, 8, 6, now.AddDays(-18)), now.AddDays(-17)), now.AddDays(-17).AddHours(2))));
        var emmaKenji = b.Completed(b.Confirmed(b.Accepted(b.MultiDay(emma, kenji, -14, 2, now.AddDays(-26)), now.AddDays(-25)), now.AddDays(-25).AddHours(5)));
        bookings.Add(emmaKenji);
        bookings.Add(b.Rejected(b.Hourly(emma, hany, 1, 10, 4, now.AddDays(-6)), now.AddDays(-5)));
        bookings.Add(b.NotPaid(b.Accepted(b.Hourly(emma, nour, -7, 9, 3, now.AddDays(-12)), now.AddDays(-11))));

        // John: more rows for the guide dashboards and ratings.
        bookings.Add(b.Pending(b.Hourly(john, omar, 9, 9, 4, now.AddHours(-3))));
        var johnOmar = b.Completed(b.Confirmed(b.Accepted(b.Hourly(john, omar, -20, 9, 5, now.AddDays(-30)), now.AddDays(-29)), now.AddDays(-29).AddHours(1)));
        bookings.Add(johnOmar);
        var johnKarim = b.Completed(b.Confirmed(b.Accepted(b.MultiDay(john, karim, -25, 2, now.AddDays(-35)), now.AddDays(-34)), now.AddDays(-34).AddHours(4)));
        bookings.Add(johnKarim);
        bookings.Add(b.Withdrawn(b.Hourly(john, yasmin, 12, 10, 3, now.AddDays(-3)), now.AddDays(-2)));
        bookings.Add(b.NoGuideResponse(b.Hourly(john, mona, 10, 9, 2, now.AddDays(-4))));

        // Sophie
        var sophieYasmin = b.Completed(b.Confirmed(b.Accepted(b.Hourly(sophie, yasmin, -10, 8, 5, now.AddDays(-20)), now.AddDays(-19)), now.AddDays(-19).AddHours(6)));
        bookings.Add(sophieYasmin);
        bookings.Add(b.Confirmed(b.Accepted(b.MultiDay(sophie, mona, 15, 2, now.AddDays(-3)), now.AddDays(-2)), now.AddDays(-2).AddHours(1)));
        bookings.Add(b.Accepted(b.Hourly(sophie, kenji, 8, 13, 3, now.AddHours(-20)), now.AddHours(-1)));

        db.Set<Booking>().AddRange(bookings);
        await db.SaveChangesAsync();

        // ---------- Payments (Confirmed + Completed) ----------
        foreach (var booking in bookings.Where(x => x.Status is BookingStatus.Confirmed or BookingStatus.Completed))
        {
            db.Set<Payment>().Add(new Payment
            {
                BookingId = booking.Id,
                Amount = booking.TotalPrice,
                TransactionReference = Guid.NewGuid().ToString(),
                PaidAtUtc = b.PaidAt[booking],
            });
        }

        // ---------- Reviews (Emma's Mona trip stays unreviewed -> "Write a review") ----------
        AddReview(db, emmaKenji, 5, "Kenji knew every hidden temple. Truly memorable.");
        AddReview(db, johnOmar, 4, "Great storyteller and very patient with our questions.");
        AddReview(db, johnKarim, 5, "Two perfect days in old Cairo. Karim found the best photo spots.");
        AddReview(db, sophieYasmin, 4, null);

        await db.SaveChangesAsync();

        // Rating totals, as the review service will keep them (FR-20).
        foreach (var guide in new[] { omar, yasmin, karim, mona, kenji, hany, nour })
        {
            var ratings = db.Set<Review>().Local.Where(r => r.GuideProfileId == guide.Id).Select(r => r.Rating).ToList();
            guide.ReviewCount = ratings.Count;
            guide.AverageRating = ratings.Count == 0 ? 0 : Math.Round((decimal)ratings.Average(), 2);
        }
        await db.SaveChangesAsync();

        await tx.CommitAsync();
        logger.LogInformation("Demo data seeded: {Users} users, {Bookings} bookings. Password for every demo account: Seed:DemoPassword.",
            await db.Users.CountAsync(), bookings.Count);
    }

    // ---------------- helpers ----------------

    private static async Task<ApplicationUser> CreateUserAsync(UserManager<ApplicationUser> users, string login, string fullName,
        string country, string role, DemoSeedSettings settings, DateTime createdAt)
    {
        var email = $"{login}@{Domain}";
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = fullName,
            CountryCode = country,
            CreatedAtUtc = createdAt,
        };
        var result = await users.CreateAsync(user, settings.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Demo user {email}: {string.Join("; ", result.Errors.Select(e => e.Description))}");
        await users.AddToRoleAsync(user, role);
        return user;
    }

    private static async Task<GuideProfile> CreateApprovedGuideAsync(UserManager<ApplicationUser> users, AppDbContext db,
        DemoSeedSettings settings, ApplicationUser admin, DateTime now, string login, string fullName, string country, string city,
        string timeZoneId, decimal pricePerHour, int years, string bio, int[] languageIds, int[] specializationIds)
    {
        var user = await CreateUserAsync(users, login, fullName, country, Roles.Guide, settings, now.AddDays(-50));
        var profile = AddGuideProfile(db, settings, user, now.AddDays(-50));
        profile.VerificationStatus = VerificationStatus.Approved;
        profile.ReviewedAtUtc = now.AddDays(-48);
        profile.ReviewedByAdminId = admin.Id;
        profile.CountryCode = country;
        profile.City = city;
        profile.TimeZoneId = timeZoneId;
        profile.Bio = bio;
        profile.YearsOfExperience = years;
        profile.PricePerHour = pricePerHour;
        foreach (var id in languageIds)
            profile.Languages.Add(new GuideLanguage { LanguageId = id });
        foreach (var id in specializationIds)
            profile.Specializations.Add(new GuideSpecialization { SpecializationId = id });
        return profile;
    }

    // New profile (Pending) with its 3 documents; the files are written outside wwwroot (NFR-02).
    private static GuideProfile AddGuideProfile(AppDbContext db, DemoSeedSettings settings, ApplicationUser user, DateTime submittedAt)
    {
        var profile = new GuideProfile
        {
            UserId = user.Id,
            VerificationStatus = VerificationStatus.Pending,
            SubmittedAtUtc = submittedAt,
        };
        foreach (var type in Enum.GetValues<DocumentType>())
        {
            var bytes = PlaceholderPdf($"{user.FullName} - {type}");
            var stored = $"{Guid.NewGuid():N}.pdf";
            Directory.CreateDirectory(settings.UploadsRootPath);
            File.WriteAllBytes(Path.Combine(settings.UploadsRootPath, stored), bytes);
            profile.Documents.Add(new GuideDocument
            {
                DocumentType = type,
                StoredFileName = stored,
                OriginalFileName = $"{type}.pdf",
                ContentType = "application/pdf",
                SizeBytes = bytes.Length,
                UploadedAtUtc = submittedAt,
            });
        }
        db.Set<GuideProfile>().Add(profile);
        return profile;
    }

    private static void AddReview(AppDbContext db, Booking booking, int rating, string? comment) =>
        db.Set<Review>().Add(new Review
        {
            BookingId = booking.Id,
            TouristId = booking.TouristId,
            GuideProfileId = booking.GuideProfileId,
            Rating = rating,
            Comment = comment,
            CreatedAtUtc = booking.CompletedAtUtc!.Value.AddHours(6),
        });

    // A one-page PDF that starts with the %PDF signature, so the content check accepts it.
    private static byte[] PlaceholderPdf(string text)
    {
        var safe = text.Replace("(", "").Replace(")", "").Replace("\\", "");
        var content = $"BT /F1 14 Tf 30 70 Td (Demo document: {safe}) Tj ET";
        var pdf =
            "%PDF-1.4\n" +
            "1 0 obj <</Type /Catalog /Pages 2 0 R>> endobj\n" +
            "2 0 obj <</Type /Pages /Kids [3 0 R] /Count 1>> endobj\n" +
            "3 0 obj <</Type /Page /Parent 2 0 R /MediaBox [0 0 595 160] /Contents 4 0 R /Resources <</Font <</F1 5 0 R>>>>>> endobj\n" +
            $"4 0 obj <</Length {content.Length}>> stream\n{content}\nendstream endobj\n" +
            "5 0 obj <</Type /Font /Subtype /Type1 /BaseFont /Helvetica>> endobj\n" +
            "trailer <</Root 1 0 R>>\n%%EOF\n";
        return Encoding.ASCII.GetBytes(pdf);
    }

    // Builds bookings with consistent prices, times and status fields (same rules as the state machine table).
    private sealed class BookingFactory
    {
        private readonly DemoSeedSettings _s;
        private readonly DateTime _now;
        public Dictionary<Booking, DateTime> PaidAt { get; } = new();

        public BookingFactory(DemoSeedSettings settings, DateTime now)
        {
            _s = settings;
            _now = now;
        }

        // dayOffset = days from the guide's local "today"; startHour = local hour.
        public Booking Hourly(ApplicationUser tourist, GuideProfile guide, int dayOffset, int startHour, int hours, DateTime createdAt)
        {
            var tz = Zone(guide.TimeZoneId!);
            var start = LocalToUtc(LocalToday(tz).AddDays(dayOffset), startHour, tz);
            return New(tourist, guide, BookingType.Hourly, start, start.AddHours(hours), hours, null, hours, createdAt);
        }

        public Booking MultiDay(ApplicationUser tourist, GuideProfile guide, int dayOffset, int days, DateTime createdAt)
        {
            var tz = Zone(guide.TimeZoneId!);
            var first = LocalToday(tz).AddDays(dayOffset);
            var start = LocalToUtc(first, 0, tz);
            var end = LocalToUtc(first.AddDays(days), 0, tz);
            return New(tourist, guide, BookingType.MultiDay, start, end, null, days, days * _s.HoursPerDay, createdAt);
        }

        public Booking Pending(Booking b) => b;

        public Booking Accepted(Booking b, DateTime respondedAt)
        {
            b.Status = BookingStatus.Accepted;
            b.RespondedAtUtc = respondedAt;
            var deadline = respondedAt.AddHours(_s.PaymentWindowHours);
            b.PaymentDeadlineUtc = deadline < b.StartUtc ? deadline : b.StartUtc;
            return b;
        }

        public Booking Rejected(Booking b, DateTime respondedAt)
        {
            b.Status = BookingStatus.Rejected;
            b.RespondedAtUtc = respondedAt;
            return b;
        }

        public Booking Confirmed(Booking b, DateTime paidAt)
        {
            b.Status = BookingStatus.Confirmed;
            PaidAt[b] = paidAt;
            return b;
        }

        public Booking Completed(Booking b)
        {
            b.Status = BookingStatus.Completed;
            b.CompletedAtUtc = b.EndUtc.AddMinutes(5);
            return b;
        }

        public Booking Withdrawn(Booking b, DateTime cancelledAt) => Cancel(b, cancelledAt, CancellationReason.WithdrawnByTourist);

        public Booking NoGuideResponse(Booking b) =>
            Cancel(b, b.CreatedAtUtc.AddHours(_s.PendingExpiryHours), CancellationReason.NoGuideResponse);

        public Booking NotPaid(Booking b) => Cancel(b, b.PaymentDeadlineUtc!.Value, CancellationReason.NotPaid);

        private static Booking Cancel(Booking b, DateTime at, CancellationReason reason)
        {
            b.Status = BookingStatus.Cancelled;
            b.CancelledAtUtc = at;
            b.CancellationReason = reason;
            return b;
        }

        private static Booking New(ApplicationUser tourist, GuideProfile guide, BookingType type, DateTime start, DateTime end,
            int? hours, int? days, int billable, DateTime createdAt)
        {
            var price = guide.PricePerHour!.Value;
            return new Booking
            {
                TouristId = tourist.Id,
                GuideProfileId = guide.Id,
                BookingType = type,
                StartUtc = start,
                EndUtc = end,
                Hours = hours,
                Days = days,
                BillableHours = billable,
                PricePerHourAtRequest = price,
                TotalPrice = billable * price,
                Status = BookingStatus.Pending,
                CreatedAtUtc = createdAt,
            };
        }

        private DateOnly LocalToday(TimeZoneInfo tz) => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(_now, tz));

        private static DateTime LocalToUtc(DateOnly day, int hour, TimeZoneInfo tz) =>
            TimeZoneInfo.ConvertTimeToUtc(day.ToDateTime(new TimeOnly(hour, 0), DateTimeKind.Unspecified), tz);

        // IANA ids work on Windows with .NET 8 (ICU); fall back to the Windows id just in case.
        private static TimeZoneInfo Zone(string ianaId)
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(ianaId); }
            catch (TimeZoneNotFoundException) when (TimeZoneInfo.TryConvertIanaIdToWindowsId(ianaId, out var winId))
            {
                return TimeZoneInfo.FindSystemTimeZoneById(winId);
            }
        }
    }
}
