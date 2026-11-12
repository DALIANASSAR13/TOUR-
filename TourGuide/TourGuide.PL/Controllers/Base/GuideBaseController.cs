using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourGuide.DAL.Constants;

namespace TourGuide.PL.Controllers.Base;

[Area("Guide")]
[Authorize(Roles = Roles.Guide)]
public abstract class GuideBaseController : Controller
{
}