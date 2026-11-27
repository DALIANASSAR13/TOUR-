using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Constants;
using TourGuide.DAL.Enums;
using TourGuide.PL.Services;

namespace TourGuide.PL.Filters;

// BR-01: a Pending or Rejected guide sees only Verification/Status in the Guide area.
public class GuideApprovalFilter : IAsyncActionFilter
{
    private readonly IGuideVerificationService _verificationService;
    private readonly ICurrentUserService _currentUser;

    public GuideApprovalFilter(
        IGuideVerificationService verificationService,
        ICurrentUserService currentUser)
    {
        _verificationService = verificationService;
        _currentUser = currentUser;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var area = context.RouteData.Values["area"]?.ToString();
        if (!string.Equals(area, "Guide", StringComparison.OrdinalIgnoreCase))
        {
            await next();
            return;
        }

        if (!context.HttpContext.User.IsInRole(Roles.Guide) || _currentUser.UserId is not { } userId)
        {
            await next();
            return;
        }

        var controller = context.RouteData.Values["controller"]?.ToString();
        var action = context.RouteData.Values["action"]?.ToString();

        // Allow VerificationController (Status, etc.) and public Photo endpoint
        if (string.Equals(controller, "Verification", StringComparison.OrdinalIgnoreCase) ||
            (string.Equals(controller, "Profile", StringComparison.OrdinalIgnoreCase) &&
             string.Equals(action, "Photo", StringComparison.OrdinalIgnoreCase)))
        {
            await next();
            return;
        }

        var statusResult = await _verificationService.GetMyStatusAsync(userId, context.HttpContext.RequestAborted);
        if (statusResult.Succeeded &&
            statusResult.Value is not null &&
            statusResult.Value.Status != VerificationStatus.Approved)
        {
            context.Result = new RedirectToActionResult("Status", "Verification", new { area = "Guide" });
            return;
        }

        await next();
    }
}