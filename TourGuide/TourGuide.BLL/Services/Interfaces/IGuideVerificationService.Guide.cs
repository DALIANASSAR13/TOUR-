using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.GuideVerification;

namespace TourGuide.BLL.Services.Interfaces;

public partial interface IGuideVerificationService
{
    Task<Result<GuideStatusDto>> GetMyStatusAsync(string userId, CancellationToken ct = default);
    Task<Result> ReapplyAsync(string userId, ReapplyRequest request, CancellationToken ct = default);
}