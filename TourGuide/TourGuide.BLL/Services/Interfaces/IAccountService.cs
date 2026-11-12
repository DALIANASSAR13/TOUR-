using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.Account;

namespace TourGuide.BLL.Services.Interfaces;

// FR-01, FR-02. Public sign-up creates only Tourist or Guide; admins are seeded.
public interface IAccountService
{
    // FR-01: creates the user with the Tourist role. Returns the new user's id.
    Task<Result<string>> RegisterTouristAsync(RegisterTouristRequest request, CancellationToken ct = default);

    // FR-02, BR-01: creates the user with the Guide role, a Pending GuideProfile and the 3 documents
    // (user, profile and documents in one transaction). Returns the new user's id.
    Task<Result<string>> RegisterGuideAsync(RegisterGuideRequest request, CancellationToken ct = default);
}
