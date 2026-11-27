using AutoMapper;
using TourGuide.BLL.DTOs.Account;
using TourGuide.DAL.Entities;

namespace TourGuide.BLL.Mappings;

// Register requests -> ApplicationUser. Copies and renames only. The password is never mapped
// (UserManager hashes it); the role and the date are set by AccountService.
// MemberList.None: ApplicationUser has many Identity properties (Id, PasswordHash, ...) that are
// not part of a request, so AutoMapper must not require them to be mapped.
public class AccountMappingProfile : Profile
{
    public AccountMappingProfile()
    {
        CreateMap<RegisterTouristRequest, ApplicationUser>(MemberList.None)
            .ForMember(u => u.UserName, o => o.MapFrom(r => r.Email));

        CreateMap<RegisterGuideRequest, ApplicationUser>(MemberList.None)
            .ForMember(u => u.UserName, o => o.MapFrom(r => r.Email));
    }
}  
