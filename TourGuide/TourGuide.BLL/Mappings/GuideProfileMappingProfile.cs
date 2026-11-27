using AutoMapper;
using TourGuide.BLL.Domain;
using TourGuide.BLL.DTOs.GuideProfile;
using TourGuide.DAL.Entities;

namespace TourGuide.BLL.Mappings;

public class GuideProfileMappingProfile : Profile
{
    public GuideProfileMappingProfile()
    {
        CreateMap<Language, LookupItemDto>()
            .ConstructUsing(s => new LookupItemDto(s.Id, s.Name));

        CreateMap<Specialization, LookupItemDto>()
            .ConstructUsing(s => new LookupItemDto(s.Id, s.Name));

        CreateMap<GuideProfile, GuideProfileEditDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.User.FullName))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.User.PhoneNumber))
            .ForMember(d => d.SelectedLanguageIds, o => o.MapFrom(s => s.Languages.Select(l => l.LanguageId).ToList()))
            .ForMember(d => d.SelectedSpecializationIds, o => o.MapFrom(s => s.Specializations.Select(sp => sp.SpecializationId).ToList()))
            .ForMember(d => d.IsBookable, o => o.MapFrom(s => GuideRules.IsBookable(s)))
            .ForMember(d => d.AvailableLanguages, o => o.Ignore())
            .ForMember(d => d.AvailableSpecializations, o => o.Ignore());
    }
}