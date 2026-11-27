using Microsoft.AspNetCore.Mvc;
using TourGuide.PL.Controllers.Base;

namespace TourGuide.PL.Areas.Admin.Controllers;

public class AdminController : AdminBaseController
{
    [HttpGet]
    public IActionResult Index() => View();
}