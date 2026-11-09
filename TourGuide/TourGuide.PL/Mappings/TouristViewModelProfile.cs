using AutoMapper;
using TourGuide.BLL.DTOs.Booking;
using TourGuide.PL.ViewModels;

namespace TourGuide.PL.Mappings;

// Tourist area: DTOs <-> ViewModels.
public class TouristViewModelProfile : Profile
{
    public TouristViewModelProfile()
    {
        CreateMap<BookingDto, BookingItemViewModel>();

        // FR-12 price preview
        CreateMap<CreateBookingViewModel, CreateBookingRequest>();
        CreateMap<PriceQuoteDto, PriceQuoteViewModel>();
    }
}
