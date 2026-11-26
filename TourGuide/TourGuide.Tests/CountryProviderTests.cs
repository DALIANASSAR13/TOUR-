using TourGuide.BLL.Infrastructure;

namespace TourGuide.Tests;

public class CountryProviderTests
{
    private readonly CountryProvider _provider = new();

    [Fact]
    public void GetAll_ReturnsCountries()
    {
        var countries = _provider.GetAll();

        Assert.NotEmpty(countries);
        Assert.All(countries, country =>
        {
            Assert.Equal(2, country.Code.Length);
            Assert.False(string.IsNullOrWhiteSpace(country.Name));
        });
    }

    [Fact]
    public void GetAll_ReturnsACopyOfTheCountryList()
    {
        var firstResult = _provider.GetAll();
        var originalCount = firstResult.Count;

        firstResult.RemoveAt(0);

        var secondResult = _provider.GetAll();

        Assert.Equal(originalCount, secondResult.Count);
    }

    [Theory]
    [InlineData("EG")]
    [InlineData("eg")]
    [InlineData(" EG ")]
    public void Exists_ReturnsTrueForExistingCountryCode(string code)
    {
        var result = _provider.Exists(code);

        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("XX")]
    [InlineData("123")]
    public void Exists_ReturnsFalseForInvalidCountryCode(string code)
    {
        var result = _provider.Exists(code);

        Assert.False(result);
    }

    [Fact]
    public void GetName_ReturnsCountryName()
    {
        var result = _provider.GetName("EG");

        Assert.Equal("Egypt", result);
    }

    [Fact]
    public void GetName_IsCaseInsensitiveAndTrimsTheCode()
    {
        var result = _provider.GetName(" eg ");

        Assert.Equal("Egypt", result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetName_ReturnsNullForEmptyCode(string? code)
    {
        var result = _provider.GetName(code);

        Assert.Null(result);
    }

    [Fact]
    public void GetName_ReturnsUnknownCodeWhenCountryDoesNotExist()
    {
        var result = _provider.GetName("XX");

        Assert.Equal("XX", result);
    }
}