using Microsoft.AspNetCore.Mvc;
using TourGuide.BLL.BusinessLogic;
using TourGuide.DAL.Data.Enums;

namespace TourGuide.PL.Controllers
{
    public class AccountController : Controller
    {

        private readonly AccountBL _accountBL  = new AccountBL();
        public AccountController()
        {
            _accountBL = new AccountBL();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }


        [HttpPost]
        public IActionResult Login(LoginViewModel LoginVM)
        {
            if (!ModelState.IsValid)
                return View(LoginVM);

            var userId = _accountBL.FindUserId(LoginVM.Email, LoginVM.Role);

            if (userId == null)
            {
                ModelState.AddModelError("", "No account found with this email for the selected role.");
                return View(LoginVM);
            }

            return LoginVM.Role switch
            {
                UserRole.Guide => RedirectToAction("Dashboard", "Guides" , new { id = userId }),
                _ => RedirectToAction("Index", "Guides")
            };

        }



    }
}
