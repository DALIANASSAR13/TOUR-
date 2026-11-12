using AutoMapper;
using TourGuide.BLL.DTOs.Search;
using TourGuide.DAL.Entities;

namespace TourGuide.BLL.Mappings;

// Seeded lookup lists -> dropdown items.
public class LookupMappingProfile : Profile
{
    public LookupMappingProfile()
    {
        CreateMap<Language, LookupDto>();
        CreateMap<Specialization, LookupDto>();
    }
}
