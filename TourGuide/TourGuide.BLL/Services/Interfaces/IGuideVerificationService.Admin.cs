using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.GuideVerification;

namespace TourGuide.BLL.Services.Interfaces;

public partial interface IGuideVerificationService
{
    Task<Result<IReadOnlyList<PendingGuideDto>>> GetPendingAsync(CancellationToken ct = default);
    Task<Result<GuideReviewDto>> GetForReviewAsync(int guideProfileId, CancellationToken ct = default);
    Task<Result> ApproveAsync(int guideProfileId, string adminUserId, CancellationToken ct = default);
    Task<Result> RejectAsync(int guideProfileId, string adminUserId, string reason, CancellationToken ct = default);
    Task<Result<DocumentStreamDto>> OpenDocumentAsync(int documentId, CancellationToken ct = default);
}