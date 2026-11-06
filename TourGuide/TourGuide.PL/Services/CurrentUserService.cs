using System.Security.Claims;

namespace TourGuide.PL.Services;

// Controllers get the logged-in user from here, never by reading claims themselves.
public interface ICurrentUserService
{
    string? UserId { get; }
    bool IsInRole(string role);
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

    public string? UserId => _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsInRole(string role) => _accessor.HttpContext?.User.IsInRole(role) ?? false;
}
