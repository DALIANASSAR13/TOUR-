using AutoMapper;
using TourGuide.BLL.Common;
using TourGuide.BLL.Domain;
using TourGuide.BLL.DTOs.Guide;
using TourGuide.BLL.DTOs.Review;
using TourGuide.BLL.Infrastructure;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Common;
using TourGuide.DAL.UnitOfWork;

namespace TourGuide.BLL.Services;

public class GuideProfileService : IGuideProfileService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly Infrastructure.ICountryProvider _countries;

    public GuideProfileService(IUnitOfWork uow, IMapper mapper, Infrastructure.ICountryProvider countries)
    {
        _uow = uow;
        _mapper = mapper;
        _countries = countries;
    }

    // FR-09. BR-02: same rule as search and booking. Read-only.
    public async Task<Result<PublicGuideProfileDto>> GetPublicProfileAsync(int guideProfileId, int reviewPage, CancellationToken ct = default)
    {
        var guide = await _uow.GuideProfiles.GetWithDetailsAsync(guideProfileId, ct);
        if (guide is null || !GuideRules.IsBookable(guide))
            return Result<PublicGuideProfileDto>.Fail(ErrorCodes.NotFound, "This guide was not found.");

        var page = Math.Max(1, reviewPage);
        var reviews = await _uow.Reviews.GetForGuideAsync(guide.Id, page, ProfileRules.ReviewPageSize, ct);

        var reviewDtos = _mapper.Map<List<ReviewDto>>(reviews.Items);
        foreach (var review in reviewDtos)
            review.TouristCountryName = _countries.GetName(review.TouristCountryCode);

        var dto = _mapper.Map<PublicGuideProfileDto>(guide);
        dto.CountryName = _countries.GetName(dto.CountryCode);
        dto.Reviews = new PagedList<ReviewDto>(reviewDtos, reviews.Page, reviews.PageSize, reviews.TotalCount);

        return Result<PublicGuideProfileDto>.Ok(dto);
    }
}
