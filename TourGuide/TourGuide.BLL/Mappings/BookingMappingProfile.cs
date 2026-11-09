using AutoMapper;
using TourGuide.BLL.DTOs.Booking;
using TourGuide.DAL.Entities;

namespace TourGuide.BLL.Mappings;

// Booking entity -> DTOs. Copies and renames only; rule-based fields are set by BookingService.
public class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<Booking, BookingDto>()
            .ForMember(d => d.GuideName, o => o.MapFrom(s => s.GuideProfile.User.FullName))
            .ForMember(d => d.GuideCity, o => o.MapFrom(s => s.GuideProfile.City))
            .ForMember(d => d.GuideCountryCode, o => o.MapFrom(s => s.GuideProfile.CountryCode))
            .ForMember(d => d.GuidePhotoFileName, o => o.MapFrom(s => s.GuideProfile.PhotoFileName))
            .ForMember(d => d.GuideTimeZoneId, o => o.MapFrom(s => s.GuideProfile.TimeZoneId))
            .ForMember(d => d.IsPaid, o => o.MapFrom(s => s.Payment != null))
            .ForMember(d => d.ReviewRating, o => o.MapFrom(s => s.Review != null ? s.Review.Rating : (int?)null))
            .ForMember(d => d.ReviewComment, o => o.MapFrom(s => s.Review != null ? s.Review.Comment : null))
            .ForMember(d => d.AutoCancelAtUtc, o => o.Ignore())
            .ForMember(d => d.CanPay, o => o.Ignore())
            .ForMember(d => d.CanWithdraw, o => o.Ignore())
            .ForMember(d => d.CanReview, o => o.Ignore());

        // FR-11: copies GuideProfileId, BookingType, Hours, Days only. Price, status, dates and the tourist
        // are set by BookingService with PriceCalculator / TimeProvider (validated by source members).
        CreateMap<CreateBookingRequest, Booking>(MemberList.Source)
            .ForSourceMember(s => s.StartDate, o => o.DoNotValidate())
            .ForSourceMember(s => s.StartTime, o => o.DoNotValidate());
    }
}
