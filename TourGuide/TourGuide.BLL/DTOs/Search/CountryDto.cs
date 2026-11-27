namespace TourGuide.BLL.DTOs.Search;

// One ISO 3166-1 alpha-2 country (ICountryProvider).
public class CountryDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
