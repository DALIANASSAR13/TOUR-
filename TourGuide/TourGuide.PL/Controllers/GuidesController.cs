using Microsoft.AspNetCore.Mvc;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Controllers
{
    // Placeholder pages until their cycles: guide search (WORK_ORDER 2.4) and the guide dashboard (2.7).
    public class GuidesController : Controller
    {
        public IActionResult Index(string? search)
        {
            return View(new GuideListViewModel { Search = search });
        }

        public IActionResult Dashboard()
        {
            return View(new GuideDashboardViewModel());
        }
    }
}
