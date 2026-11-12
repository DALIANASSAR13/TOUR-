using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourGuide.DAL.Constants;

namespace TourGuide.PL.Controllers.Base;

[Area("Tourist")]
[Authorize(Roles = Roles.Tourist)]
public abstract class TouristBaseController : Controller
{
}
