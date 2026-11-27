using AutoMapper;
using TourGuide.BLL.DTOs.Review;
using TourGuide.DAL.Entities;

namespace TourGuide.BLL.Mappings;

// Review entity <-> DTOs. Copies and renames only.
public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.TouristName, o => o.MapFrom(s => s.Tourist.FullName))
            .ForMember(d => d.TouristCountryCode, o => o.MapFrom(s => s.Tourist.CountryCode))
            .ForMember(d => d.TouristCountryName, o => o.Ignore());

        // FR-19: copies BookingId, Rating and Comment only. The tourist, the guide and the date
        // are set by ReviewService from the booking and TimeProvider (validated by source members).
        CreateMap<CreateReviewRequest, Review>(MemberList.Source);
    }
}
