using AutoMapper;
using TourGuide.BLL.DTOs.Payment;
using TourGuide.DAL.Entities;

namespace TourGuide.BLL.Mappings;

// Booking entity -> payment DTOs. Copies and renames only.
public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<Booking, CheckoutDto>()
            .ForMember(d => d.GuideName, o => o.MapFrom(s => s.GuideProfile.User.FullName))
            .ForMember(d => d.GuideCity, o => o.MapFrom(s => s.GuideProfile.City))
            .ForMember(d => d.GuideCountryCode, o => o.MapFrom(s => s.GuideProfile.CountryCode))
            .ForMember(d => d.GuideTimeZoneId, o => o.MapFrom(s => s.GuideProfile.TimeZoneId));
    }
}
