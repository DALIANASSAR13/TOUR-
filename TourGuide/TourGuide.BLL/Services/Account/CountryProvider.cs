using TourGuide.BLL.Services.Interfaces;

namespace TourGuide.BLL.Services.Account;

public class CountryProvider : ICountryProvider
{
    private static readonly List<CountryOption> Countries =
    [
        new("EG", "Egypt"),
        new("US", "United States"),
        new("GB", "United Kingdom"),
        new("CA", "Canada"),
        new("AU", "Australia"),
        new("FR", "France"),
        new("DE", "Germany"),
        new("IT", "Italy"),
        new("ES", "Spain"),
        new("SA", "Saudi Arabia"),
        new("AE", "United Arab Emirates"),
        new("JO", "Jordan"),
        new("MA", "Morocco"),
        new("TN", "Tunisia"),
        new("TR", "Turkey"),
        new("JP", "Japan"),
        new("CN", "China"),
        new("IN", "India")
    ];

    public IReadOnlyList<CountryOption> GetCountries()
    {
        return Countries;
    }

    public bool IsValidCountry(string countryCode)
    {
        return Countries.Any(c =>
            c.Code.Equals(
                countryCode,
                StringComparison.OrdinalIgnoreCase));
    }
}
