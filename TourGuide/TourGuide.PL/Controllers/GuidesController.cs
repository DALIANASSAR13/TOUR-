using Microsoft.AspNetCore.Mvc;
using TourGuide.DAL.Entities;
using TourGuide.BLL.BusinessLogic;

namespace TourGuide.PL.Controllers
{
    public class GuidesController : Controller
    {
        private readonly GuidesBL _guideBL = new GuidesBL();
        public IActionResult Index(string? search)
        {
            var GuidesList = _guideBL.GetGuidesList(search);
            return View(GuidesList);
        }
    }
}
