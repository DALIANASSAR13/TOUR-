using Microsoft.AspNetCore.Mvc;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // Real sign-in with SignInManager comes in WORK_ORDER cycle 1.1.a.
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
                ModelState.AddModelError(string.Empty, "Sign-in is not connected yet (cycle 1.1.a).");

            return View(model);
        }
    }
}
