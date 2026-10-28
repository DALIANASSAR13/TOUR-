using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TourGuide.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserType",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, "role-guide-0001", "Guide", "GUIDE" },
                    { 2, "role-tourist-0002", "Tourist", "TOURIST" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AboutMe", "AccessFailedCount", "ConcurrencyStamp", "Country", "CreatedAt", "Email", "EmailConfirmed", "FullName", "GoogleId", "IsVerified", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PricePerHour", "ProfileImageUrl", "Role", "SecurityStamp", "Title", "ToursCompleted", "TwoFactorEnabled", "UserName", "UserType" },
                values: new object[,]
                {
                    { 1, "Licensed Egyptologist with over 10 years of experience guiding visitors through the Giza Plateau, Saqqara and Old Cairo.", 0, "concurrency-user-0001", "Egypt", new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), "ahmed.hassan@tourguide.com", true, "Ahmed Hassan", null, true, true, null, "AHMED.HASSAN@TOURGUIDE.COM", "AHMED.HASSAN@TOURGUIDE.COM", "PASTE_GENERATED_HASH_HERE", null, false, 25.00m, "/images/guides/ahmed.jpg", "Guide", "stamp-user-0001", "Egyptologist & Cairo/Giza Guide", 120, false, "ahmed.hassan@tourguide.com", "Guide" },
                    { 2, "Born and raised in Luxor. I love sharing the stories behind Karnak, Luxor Temple and the Valley of the Kings.", 0, "concurrency-user-0002", "Egypt", new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), "mariam.adel@tourguide.com", true, "Mariam Adel", null, true, true, null, "MARIAM.ADEL@TOURGUIDE.COM", "MARIAM.ADEL@TOURGUIDE.COM", "PASTE_GENERATED_HASH_HERE", null, false, 22.00m, "/images/guides/mariam.jpg", "Guide", "stamp-user-0002", "Luxor & West Bank Specialist", 85, false, "mariam.adel@tourguide.com", "Guide" },
                    { 3, "Nubian guide who knows every corner of Aswan, from Philae Temple to the colorful villages along the Nile.", 0, "concurrency-user-0003", "Egypt", new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), "omar.khaled@tourguide.com", true, "Omar Khaled", null, true, true, null, "OMAR.KHALED@TOURGUIDE.COM", "OMAR.KHALED@TOURGUIDE.COM", "PASTE_GENERATED_HASH_HERE", null, false, 20.00m, "/images/guides/omar.jpg", "Guide", "stamp-user-0003", "Aswan & Nubian Culture Expert", 60, false, "omar.khaled@tourguide.com", "Guide" },
                    { 4, "History graduate who loves Alexandria's Greco-Roman past, seaside cafes and hidden local spots.", 0, "concurrency-user-0004", "Egypt", new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), "nour.salah@tourguide.com", true, "Nour Salah", null, false, true, null, "NOUR.SALAH@TOURGUIDE.COM", "NOUR.SALAH@TOURGUIDE.COM", "PASTE_GENERATED_HASH_HERE", null, false, 18.00m, "/images/guides/nour.jpg", "Guide", "stamp-user-0004", "Alexandria Heritage Guide", 40, false, "nour.salah@tourguide.com", "Guide" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Country", "CreatedAt", "Email", "EmailConfirmed", "FullName", "GoogleId", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfileImageUrl", "Role", "SecurityStamp", "TwoFactorEnabled", "UserName", "UserType" },
                values: new object[,]
                {
                    { 5, 0, "concurrency-user-0005", "United Kingdom", new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), "emily.carter@example.com", true, "Emily Carter", null, true, null, "EMILY.CARTER@EXAMPLE.COM", "EMILY.CARTER@EXAMPLE.COM", "PASTE_GENERATED_HASH_HERE", null, false, null, "Tourist", "stamp-user-0005", false, "emily.carter@example.com", "Tourist" },
                    { 6, 0, "concurrency-user-0006", "Germany", new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), "lucas.meyer@example.com", true, "Lucas Meyer", null, true, null, "LUCAS.MEYER@EXAMPLE.COM", "LUCAS.MEYER@EXAMPLE.COM", "PASTE_GENERATED_HASH_HERE", null, false, null, "Tourist", "stamp-user-0006", false, "lucas.meyer@example.com", "Tourist" },
                    { 7, 0, "concurrency-user-0007", "Italy", new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), "sofia.rossi@example.com", true, "Sofia Rossi", null, true, null, "SOFIA.ROSSI@EXAMPLE.COM", "SOFIA.ROSSI@EXAMPLE.COM", "PASTE_GENERATED_HASH_HERE", null, false, null, "Tourist", "stamp-user-0007", false, "sofia.rossi@example.com", "Tourist" },
                    { 8, 0, "concurrency-user-0008", "South Korea", new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), "daniel.kim@example.com", true, "Daniel Kim", null, true, null, "DANIEL.KIM@EXAMPLE.COM", "DANIEL.KIM@EXAMPLE.COM", "PASTE_GENERATED_HASH_HERE", null, false, null, "Tourist", "stamp-user-0008", false, "daniel.kim@example.com", "Tourist" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 2, 5 },
                    { 2, 6 },
                    { 2, 7 },
                    { 2, 8 }
                });

            migrationBuilder.InsertData(
                table: "CustomTripRequests",
                columns: new[] { "Id", "Budget", "CreatedAt", "EndDate", "GuideId", "StartDate", "Status", "TouristId" },
                values: new object[,]
                {
                    { 1, 1500.00m, new DateTime(2026, 10, 26, 13, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 3, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2027, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Requested", 7 },
                    { 2, 1200.00m, new DateTime(2026, 10, 18, 16, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2027, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "InProgress", 8 }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "NotificationType", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 20, 12, 1, 0, 0, DateTimeKind.Unspecified), true, "New booking request for Pyramids of Giza & Sphinx Day Tour (2 people).", "NewBookingRequest", 1 },
                    { 2, new DateTime(2026, 10, 25, 15, 31, 0, 0, DateTimeKind.Unspecified), false, "New booking request for Pyramids of Giza & Sphinx Day Tour (3 people).", "NewBookingRequest", 1 },
                    { 3, new DateTime(2026, 10, 22, 9, 16, 0, 0, DateTimeKind.Unspecified), false, "New booking request for Luxor East & West Bank Highlights (4 people).", "NewBookingRequest", 2 },
                    { 4, new DateTime(2026, 9, 10, 10, 1, 0, 0, DateTimeKind.Unspecified), true, "New booking request for Luxor Sunrise Hot Air Balloon (2 people).", "NewBookingRequest", 2 }
                });

            migrationBuilder.InsertData(
                table: "Tours",
                columns: new[] { "Id", "AverageRating", "Description", "DurationDays", "EndDate", "GuideId", "ImageUrl", "IsActive", "MaxGroupSize", "PricePerPerson", "SlotsLeft", "StartDate", "Title", "Type" },
                values: new object[,]
                {
                    { 1, 0.0, "Walk the Giza Plateau with an Egyptologist: the Great Pyramid, the Sphinx, the Valley Temple and the best panoramic viewpoint.", 1, new DateTime(2026, 12, 12, 16, 0, 0, 0, DateTimeKind.Unspecified), 1, "/images/tours/pyramids.jpg", true, 12, 60.00m, 7, new DateTime(2026, 12, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "Pyramids of Giza & Sphinx Day Tour", "Historical" },
                    { 2, 0.0, "Two days across Luxor: Karnak and Luxor Temples on the East Bank, then the Valley of the Kings and Hatshepsut Temple on the West Bank.", 2, new DateTime(2027, 1, 16, 17, 0, 0, 0, DateTimeKind.Unspecified), 2, "/images/tours/luxor.jpg", true, 10, 140.00m, 6, new DateTime(2027, 1, 15, 8, 0, 0, 0, DateTimeKind.Unspecified), "Luxor East & West Bank Highlights", "Historical" },
                    { 3, 0.0, "Felucca ride on the Nile, Philae Temple, the High Dam and a home-cooked lunch in a Nubian village.", 3, new DateTime(2027, 2, 7, 18, 0, 0, 0, DateTimeKind.Unspecified), 3, "/images/tours/aswan.jpg", true, 8, 210.00m, 8, new DateTime(2027, 2, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), "Aswan, Philae & Nubian Village Experience", "Cruise" },
                    { 4, 0.0, "Stroll the Corniche, visit the Bibliotheca Alexandrina, Qaitbay Citadel and the Roman Amphitheatre, finishing with fresh seafood.", 1, new DateTime(2027, 3, 6, 17, 0, 0, 0, DateTimeKind.Unspecified), 4, "/images/tours/alexandria.jpg", true, 15, 45.00m, 15, new DateTime(2027, 3, 6, 9, 0, 0, 0, DateTimeKind.Unspecified), "Alexandria Coastal Heritage Walk", "Walking" },
                    { 5, 4.5, "Coptic Cairo, Al-Muizz Street and the bazaar after sunset, with street-food tastings along the way.", 1, new DateTime(2026, 9, 12, 22, 0, 0, 0, DateTimeKind.Unspecified), 1, "/images/tours/old-cairo.jpg", false, 10, 40.00m, 6, new DateTime(2026, 9, 12, 16, 0, 0, 0, DateTimeKind.Unspecified), "Old Cairo & Khan el-Khalili Evening", "Walking" },
                    { 6, 5.0, "Float over the Nile and the temples of Luxor at sunrise, followed by breakfast on the West Bank.", 1, new DateTime(2026, 10, 2, 10, 0, 0, 0, DateTimeKind.Unspecified), 2, "/images/tours/balloon.jpg", false, 6, 95.00m, 4, new DateTime(2026, 10, 2, 5, 0, 0, 0, DateTimeKind.Unspecified), "Luxor Sunrise Hot Air Balloon", "Photography" }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "CreatedAt", "NumberOfPeople", "Status", "TotalPrice", "TourId", "TouristId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 20, 12, 0, 0, 0, DateTimeKind.Unspecified), 2, "Confirmed", 120.00m, 1, 5 },
                    { 2, new DateTime(2026, 10, 25, 15, 30, 0, 0, DateTimeKind.Unspecified), 3, "Pending", 180.00m, 1, 8 },
                    { 3, new DateTime(2026, 10, 22, 9, 15, 0, 0, DateTimeKind.Unspecified), 4, "Confirmed", 560.00m, 2, 6 },
                    { 4, new DateTime(2026, 8, 30, 11, 0, 0, 0, DateTimeKind.Unspecified), 2, "Completed", 80.00m, 5, 5 },
                    { 5, new DateTime(2026, 9, 1, 14, 0, 0, 0, DateTimeKind.Unspecified), 2, "Completed", 80.00m, 5, 6 },
                    { 6, new DateTime(2026, 9, 10, 10, 0, 0, 0, DateTimeKind.Unspecified), 2, "Completed", 190.00m, 6, 7 }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "Rating", "TourId", "TouristId" },
                values: new object[,]
                {
                    { 1, "Ahmed made Old Cairo come alive. The food stops were the highlight of our trip!", new DateTime(2026, 9, 14, 10, 0, 0, 0, DateTimeKind.Unspecified), 5.0, 5, 5 },
                    { 2, "Great stories and a well-paced evening. It got a little crowded in the bazaar.", new DateTime(2026, 9, 15, 18, 0, 0, 0, DateTimeKind.Unspecified), 4.0, 5, 6 },
                    { 3, "Unforgettable sunrise over Luxor. Mariam organized everything perfectly.", new DateTime(2026, 10, 3, 9, 0, 0, 0, DateTimeKind.Unspecified), 5.0, 6, 7 }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "BookingId", "GuideFee", "Method", "PaidAt", "PlatformFee", "Status" },
                values: new object[,]
                {
                    { 1, 1, 108.00m, "CreditCard", new DateTime(2026, 10, 20, 12, 5, 0, 0, DateTimeKind.Unspecified), 12.00m, "HeldInEscrow" },
                    { 2, 2, 162.00m, "PayPal", null, 18.00m, "Pending" },
                    { 3, 3, 504.00m, "ApplePay", new DateTime(2026, 10, 22, 9, 20, 0, 0, DateTimeKind.Unspecified), 56.00m, "HeldInEscrow" },
                    { 4, 4, 72.00m, "CreditCard", new DateTime(2026, 8, 30, 11, 5, 0, 0, DateTimeKind.Unspecified), 8.00m, "Paid" },
                    { 5, 5, 72.00m, "PayPal", new DateTime(2026, 9, 1, 14, 5, 0, 0, DateTimeKind.Unspecified), 8.00m, "Paid" },
                    { 6, 6, 171.00m, "CreditCard", new DateTime(2026, 9, 10, 10, 5, 0, 0, DateTimeKind.Unspecified), 19.00m, "Paid" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 5 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 6 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 7 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 8 });

            migrationBuilder.DeleteData(
                table: "CustomTripRequests",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CustomTripRequests",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Notifications",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tours",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tours",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Tours",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tours",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tours",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Tours",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AlterColumn<string>(
                name: "UserType",
                table: "AspNetUsers",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);
        }
    }
}
