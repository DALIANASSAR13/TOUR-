using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.Guide;

namespace TourGuide.BLL.Services.Interfaces;

// FR-08, FR-09. Cycle 2.3 adds GetMyProfileAsync, UpdateMyProfileAsync and GetPhotoAsync.
public interface IGuideProfileService
{
    // FR-09: public profile of an Approved guide with a complete profile (BR-02), with one page of reviews.
    // Any other guide -> NotFound.
    Task<Result<PublicGuideProfileDto>> GetPublicProfileAsync(int guideProfileId, int reviewPage, CancellationToken ct = default);
}
