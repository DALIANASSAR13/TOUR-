using FluentValidation.TestHelper;
using TourGuide.BLL.DTOs.Account;
using TourGuide.BLL.Infrastructure;
using TourGuide.BLL.Validators;

namespace TourGuide.Tests;

public class RegisterTouristValidatorTests
{
    private readonly RegisterTouristValidator _validator =
        new(new CountryProvider());

    [Fact]
    public void ValidRequest_PassesValidation()
    {
        var request = new RegisterTouristRequest
        {
            FullName = "John Smith",
            Email = "john@example.com",
            Password = "Password123",
            CountryCode = "EG"
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void InvalidCountryCode_FailsValidation()
    {
        var request = new RegisterTouristRequest
        {
            FullName = "John Smith",
            Email = "john@example.com",
            Password = "Password123",
            CountryCode = "XX"
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.CountryCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyCountryCode_FailsValidation(string countryCode)
    {
        var request = new RegisterTouristRequest
        {
            FullName = "John Smith",
            Email = "john@example.com",
            Password = "Password123",
            CountryCode = countryCode
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.CountryCode);
    }
}