using AutoMapper;
using TourGuide.BLL.Domain;
using TourGuide.BLL.DTOs.GuideVerification;
using TourGuide.DAL.Entities;

namespace TourGuide.BLL.Mappings;

public class GuideVerificationMappingProfile : Profile
{
    public GuideVerificationMappingProfile()
    {
        CreateMap<GuideProfile, GuideStatusDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.VerificationStatus))
            .ForMember(d => d.IsProfileComplete, o => o.MapFrom(s => GuideRules.IsBookable(s)));
    }
}