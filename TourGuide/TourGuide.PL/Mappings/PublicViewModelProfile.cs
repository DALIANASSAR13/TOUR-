using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using TourGuide.BLL.DTOs.Guide;
using TourGuide.BLL.DTOs.Review;
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

        // FR-09 public profile: the reviews page is flattened onto the page model.
        CreateMap<ReviewDto, ReviewItemViewModel>();
        CreateMap<PublicGuideProfileDto, PublicGuideProfileViewModel>()
            .ForMember(d => d.Reviews, o => o.MapFrom(s => s.Reviews.Items))
            .ForMember(d => d.ReviewPage, o => o.MapFrom(s => s.Reviews.Page))
            .ForMember(d => d.ReviewTotalPages, o => o.MapFrom(s => s.Reviews.TotalPages))
            .ForMember(d => d.CanBook, o => o.Ignore());

        CreateMap<CountryDto, SelectListItem>(MemberList.None)
            .ForMember(d => d.Value, o => o.MapFrom(s => s.Code))
            .ForMember(d => d.Text, o => o.MapFrom(s => s.Name));
        CreateMap<LookupDto, SelectListItem>(MemberList.None)
            .ForMember(d => d.Value, o => o.MapFrom(s => s.Id.ToString()))
            .ForMember(d => d.Text, o => o.MapFrom(s => s.Name));
    }
}
