using AutoMapper;
using TourGuide.BLL.DTOs.Booking;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Mappings;

// Guide area: DTOs <-> ViewModels.
public class GuideViewModelProfile : Profile
{
    public GuideViewModelProfile()
    {
        // FR-16 requests list
        CreateMap<BookingDto, GuideBookingItemViewModel>();
    }
}
