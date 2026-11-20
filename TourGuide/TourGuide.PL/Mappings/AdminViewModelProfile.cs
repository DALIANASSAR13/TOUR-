using AutoMapper;
using TourGuide.BLL.DTOs.GuideVerification;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Mappings;

public class AdminViewModelProfile : Profile
{
    public AdminViewModelProfile()
    {
        CreateMap<PendingGuideDto, PendingGuideItemViewModel>();
    }
}