using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TourGuide.DAL.Constants;
using TourGuide.DAL.Entities;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Controllers;

// TEMPORARY, Development only: sign in as a demo user with one click, so pages can be tested
// before real login exists (Cycle 1.1.a). Returns 404 outside Development.
// Remove this controller, its view and the layout link when 1.1.a is done (STATE.md -> Known issues).
public class DevLoginController : Controller
{
    private static readonly DevLoginUserViewModel[] DemoUsers =
    {
        new("emma.schmidt@tourguide.local", "Emma Schmidt", Roles.Tourist, "7 bookings, one per status (UI screen 6)"),
        new("john.carter@tourguide.local", "John Carter", Roles.Tourist, "5 bookings"),
        new("sophie.martin@tourguide.local", "Sophie Martin", Roles.Tourist, "3 bookings"),
        new("omar.hassan@tourguide.local", "Omar Hassan", Roles.Guide, "Approved guide: Pending, overlapping Pending, Accepted, Completed"),
        new("mona.fawzy@tourguide.local", "Mona Fawzy", Roles.Guide, "Approved guide: Confirmed, Completed, Cancelled"),
        new("ahmed.tarek@tourguide.local", "Ahmed Tarek", Roles.Guide, "Pending guide"),
        new("laila.mostafa@tourguide.local", "Laila Mostafa", Roles.Guide, "Rejected guide"),
        new("admin@tourguide.local", "Demo Admin", Roles.Admin, "Only if no admin was set in user secrets"),
    };

    private readonly IWebHostEnvironment _env;
    private readonly UserManager<ApplicationUser> _users;
    private readonly SignInManager<ApplicationUser> _signIn;

    public DevLoginController(IWebHostEnvironment env, UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn)
    {
        _env = env;
        _users = users;
        _signIn = signIn;
    }

    [HttpGet]
    public IActionResult Index(string? returnUrl)
    {
        if (!_env.IsDevelopment())
            return NotFound();
        ViewData["ReturnUrl"] = returnUrl;
        return View(DemoUsers);
    }

    [HttpPost]
    public async Task<IActionResult> As(string email, string? returnUrl)
    {
        if (!_env.IsDevelopment())
            return NotFound();

        var user = await _users.FindByEmailAsync(email);
        if (user is null)
        {
            TempData["DevLoginError"] = $"No user {email}. Did the demo seed run?";
            return RedirectToAction(nameof(Index), new { returnUrl });
        }

        await _signIn.SignInAsync(user, isPersistent: false);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);
        if (await _users.IsInRoleAsync(user, Roles.Tourist))
            return RedirectToAction("Index", "Bookings", new { area = "Tourist" });
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        if (!_env.IsDevelopment())
            return NotFound();
        await _signIn.SignOutAsync();
        return RedirectToAction(nameof(Index));
    }
}
