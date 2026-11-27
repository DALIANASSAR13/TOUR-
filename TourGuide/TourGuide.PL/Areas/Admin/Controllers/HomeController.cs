using Microsoft.AspNetCore.Mvc;
using TourGuide.PL.Controllers.Base;

namespace TourGuide.PL.Areas.Admin.Controllers;

public class HomeController : AdminBaseController
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "GuideVerification", new { area = "Admin" });
    }
}