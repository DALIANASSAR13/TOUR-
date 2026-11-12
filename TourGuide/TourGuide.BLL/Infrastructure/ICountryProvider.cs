using TourGuide.BLL.DTOs.Search;

namespace TourGuide.BLL.Infrastructure;

// ISO country list for dropdowns and validation (FR-01, FR-08, FR-10).
public interface ICountryProvider
{
    List<CountryDto> GetAll();
    bool Exists(string code);

    // English name for a code, or the code itself when unknown (display only).
    string? GetName(string? code);
}
