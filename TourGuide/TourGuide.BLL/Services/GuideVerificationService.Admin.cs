using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.GuideVerification;

namespace TourGuide.BLL.Services;

public partial class GuideVerificationService
{
    public async Task<Result<IReadOnlyList<PendingGuideDto>>> GetPendingAsync(CancellationToken ct = default)
    {
        var pendingGuides = await _uow.GuideProfiles.GetPendingAsync(ct);
        var dtos = _mapper.Map<IReadOnlyList<PendingGuideDto>>(pendingGuides);
        return Result<IReadOnlyList<PendingGuideDto>>.Ok(dtos);
    }
}