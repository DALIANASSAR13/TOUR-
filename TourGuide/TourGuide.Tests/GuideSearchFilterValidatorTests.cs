using TourGuide.BLL.DTOs.Search;
using TourGuide.BLL.Infrastructure;
using TourGuide.BLL.Validators;

namespace TourGuide.Tests;

public class GuideSearchFilterValidatorTests
{
    private readonly GuideSearchFilterValidator _validator =
        new(new CountryProvider());

    [Fact]
    public async Task Valid_filter_passes_validation()
    {
        var filter = new GuideSearchFilter
        {
            CountryCode = "EG",
            LanguageId = 1,
            SpecializationId = 1,
            MinPrice = 10,
            MaxPrice = 50,
            Page = 1
        };

        var result = await _validator.ValidateAsync(filter);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Min_price_greater_than_max_price_fails_validation()
    {
        var filter = new GuideSearchFilter
        {
            MinPrice = 60,
            MaxPrice = 50
        };

        var result = await _validator.ValidateAsync(filter);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage.Contains("Min price"));
    }

    [Fact]
    public async Task Unknown_country_fails_validation()
    {
        var filter = new GuideSearchFilter { CountryCode = "XX" };

        var result = await _validator.ValidateAsync(filter);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.ErrorMessage.Contains("country", StringComparison.OrdinalIgnoreCase));
    }
}
