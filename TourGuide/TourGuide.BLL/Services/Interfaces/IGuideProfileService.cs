using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.GuideProfile;

namespace TourGuide.BLL.Services.Interfaces;

public interface IGuideProfileService
{
    Task<Result<GuideProfileEditDto>> GetForEditAsync(string userId, CancellationToken ct = default);
    Task<Result<GuideProfileEditDto>> UpdateAsync(string userId, UpdateGuideProfileRequest request, CancellationToken ct = default);
}
