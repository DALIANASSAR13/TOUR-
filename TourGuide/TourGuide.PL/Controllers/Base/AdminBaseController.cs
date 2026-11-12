using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourGuide.DAL.Constants;

namespace TourGuide.PL.Controllers.Base;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public abstract class AdminBaseController : Controller
{
}