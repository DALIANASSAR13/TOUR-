using AutoMapper;
using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.GuideVerification;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.UnitOfWork;

namespace TourGuide.BLL.Services;

public partial class GuideVerificationService : IGuideVerificationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GuideVerificationService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<GuideStatusDto>> GetMyStatusAsync(string userId, CancellationToken ct = default)
    {
        var profile = await _uow.GuideProfiles.GetByUserIdAsync(userId, ct);
        if (profile is null)
            return Result<GuideStatusDto>.Fail(ErrorCodes.NotFound, "Guide profile was not found.");

        var dto = _mapper.Map<GuideStatusDto>(profile);
        return Result<GuideStatusDto>.Ok(dto);
    }
}