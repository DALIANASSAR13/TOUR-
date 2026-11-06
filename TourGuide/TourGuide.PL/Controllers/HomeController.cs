using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TourGuide.PL.Models;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Controllers
{
    public class HomeController : Controller
    {
        // Tours were removed (out of scope in the SRS). The home page shows top-rated guides
        // once search is built (WORK_ORDER 2.4); until then the list is empty.
        public IActionResult Index()
        {
            return View(new HomeViewModel());
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
