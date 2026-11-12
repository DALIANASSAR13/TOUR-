namespace TourGuide.BLL.DTOs.Search;

// The choices for the FR-10 filter dropdowns.
public class SearchFilterOptionsDto
{
    public List<CountryDto> Countries { get; set; } = new();
    public List<LookupDto> Languages { get; set; } = new();
    public List<LookupDto> Specializations { get; set; } = new();
}
