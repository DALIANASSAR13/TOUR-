using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TourGuide.BLL.DTOs.Search;
using TourGuide.DAL.Common;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Mappings;

// Public pages (no area): DTOs <-> ViewModels.
public class PublicViewModelProfile : Profile
{
    public PublicViewModelProfile()
    {
        // FR-10 search
        CreateMap<SearchViewModel, GuideSearchFilter>();
        CreateMap<GuideCardDto, GuideCardViewModel>();

        // Filled into the existing page model: only the source members are checked.
        CreateMap<PagedList<GuideCardDto>, SearchViewModel>(MemberList.Source)
            .ForMember(d => d.Guides, o => o.MapFrom(s => s.Items))
            .ForSourceMember(s => s.Items, o => o.DoNotValidate());
        CreateMap<SearchFilterOptionsDto, SearchViewModel>(MemberList.Source);

        CreateMap<CountryDto, SelectListItem>(MemberList.None)
            .ForMember(d => d.Value, o => o.MapFrom(s => s.Code))
            .ForMember(d => d.Text, o => o.MapFrom(s => s.Name));
        CreateMap<LookupDto, SelectListItem>(MemberList.None)
            .ForMember(d => d.Value, o => o.MapFrom(s => s.Id.ToString()))
            .ForMember(d => d.Text, o => o.MapFrom(s => s.Name));
    }
}
