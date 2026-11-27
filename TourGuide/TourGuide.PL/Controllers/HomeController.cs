using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TourGuide.PL.Models;

namespace TourGuide.PL.Controllers
{
    public class HomeController : Controller
    {
        // Static landing page (UI screen 1). Top-rated guides are added with search (WORK_ORDER 2.4).
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
