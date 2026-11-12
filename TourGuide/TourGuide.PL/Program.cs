using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TourGuide.BLL;
using TourGuide.BLL.Domain;
using TourGuide.BLL.Options;
using TourGuide.BLL.Services.Account;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL;
using TourGuide.DAL.Data;
using TourGuide.DAL.Data.Seed;
using TourGuide.DAL.Entities;
using TourGuide.PL.Services;

var builder = WebApplication.CreateBuilder(args);

// Layers: DAL (DbContext, repositories, Unit of Work) and BLL (services, validators, options).
builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddBusinessLogic(builder.Configuration);
// Uploaded files live outside wwwroot (NFR-02). A relative RootPath is placed under the content root.
builder.Services.PostConfigure<FileStorageOptions>(options =>
    options.RootPath = Path.Combine(builder.Environment.ContentRootPath, options.RootPath));

// AutoMapper profiles from BLL (entity <-> DTO) and PL (DTO <-> ViewModel).
builder.Services.AddAutoMapper(typeof(TourGuide.BLL.DependencyInjection).Assembly, typeof(Program).Assembly);

// Identity: hashed passwords (NFR-03) and roles (NFR-01).
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
// // 
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";

    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Every POST must carry the anti-forgery token.
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // A missing or wrong mapping fails here, at startup.
    app.Services.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Admin account (roles, languages and specializations are seeded by the migration).
try
{
    await AdminSeeder.SeedAsync(app.Services, app.Configuration, app.Logger);

    // Demo data for every table, Development only and only when Seed:DemoData is true.
    if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("Seed:DemoData"))
    {
        var storage = app.Configuration.GetSection(FileStorageOptions.SectionName).Get<FileStorageOptions>() ?? new FileStorageOptions();
        await DemoDataSeeder.SeedAsync(app.Services, new DemoSeedSettings(
            Password: app.Configuration["Seed:DemoPassword"]
                      ?? throw new InvalidOperationException("Set Seed:DemoPassword to seed demo data."),
            UploadsRootPath: Path.Combine(app.Environment.ContentRootPath, storage.RootPath),
            HoursPerDay: BookingRules.HoursPerDay,
            PendingExpiryHours: BookingRules.PendingExpiryHours,
            PaymentWindowHours: BookingRules.PaymentWindowHours), app.Logger);
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Seeding failed. Did you run 'dotnet ef database update'?");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
