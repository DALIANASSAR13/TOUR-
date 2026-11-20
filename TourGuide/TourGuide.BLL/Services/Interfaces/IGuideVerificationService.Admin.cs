using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.GuideVerification;

namespace TourGuide.BLL.Services.Interfaces;

public partial interface IGuideVerificationService
{
    Task<Result<IReadOnlyList<PendingGuideDto>>> GetPendingAsync(CancellationToken ct = default);
}