using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TourGuide.BLL.BusinessLogic;
using TourGuide.PL.Models;

namespace TourGuide.PL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ToursBL _toursBL;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _toursBL = new ToursBL();
        }

        public IActionResult Index()
        {
            var homeData = _toursBL.GetHomeData();
            return View(homeData);
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
