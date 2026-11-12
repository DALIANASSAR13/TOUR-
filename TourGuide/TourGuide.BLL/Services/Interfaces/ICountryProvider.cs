namespace TourGuide.BLL.Services.Interfaces;

public interface ICountryProvider
{
    IReadOnlyList<CountryOption> GetCountries();

    bool IsValidCountry(string countryCode);
}

public record CountryOption(string Code, string Name);
