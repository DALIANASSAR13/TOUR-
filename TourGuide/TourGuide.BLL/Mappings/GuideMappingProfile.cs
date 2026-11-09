using AutoMapper;
using TourGuide.BLL.DTOs.Search;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Queries;

namespace TourGuide.BLL.Mappings;

// GuideProfile <-> guide DTOs. Copies and renames only.
public class GuideMappingProfile : Profile
{
    public GuideMappingProfile()
    {
        // FR-10 result card. CountryName is looked up by SearchService (ICountryProvider).
        CreateMap<GuideProfile, GuideCardDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.User.FullName))
            .ForMember(d => d.PricePerHour, o => o.MapFrom(s => s.PricePerHour.GetValueOrDefault()))
            .ForMember(d => d.Languages, o => o.MapFrom(s => s.Languages.Select(l => l.Language.Name).OrderBy(n => n)))
            .ForMember(d => d.Specializations, o => o.MapFrom(s => s.Specializations.Select(x => x.Specialization.Name).OrderBy(n => n)))
            .ForMember(d => d.CountryName, o => o.Ignore());

        // FR-10 filter -> DAL query. PageSize comes from SearchRules, set by SearchService.
        CreateMap<GuideSearchFilter, GuideSearchQuery>()
            .ForMember(d => d.PageSize, o => o.Ignore());
    }
}
