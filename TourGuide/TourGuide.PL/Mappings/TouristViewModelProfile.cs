using AutoMapper;
using TourGuide.BLL.DTOs.Booking;
using TourGuide.BLL.DTOs.Payment;
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

        // FR-14 checkout: every CheckoutDto field has the same name on the ViewModel (checked by source members);
        // the other card fields (review, allowed actions) are not used on this page.
        CreateMap<CheckoutDto, CheckoutViewModel>(MemberList.Source);
    }
}
