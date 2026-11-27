using TourGuide.BLL.Domain;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Enums;

namespace TourGuide.Tests;

public class GuideRulesTests
{
    [Fact]
    public void Approved_complete_profile_is_bookable()
    {
        var profile = new GuideProfile
        {
            VerificationStatus = VerificationStatus.Approved,
            CountryCode = "EG",
            City = "Cairo",
            TimeZoneId = "Africa/Cairo",
            PricePerHour = 25m,
            Languages = [new GuideLanguage()],
            Specializations = [new GuideSpecialization()]
        };

        Assert.True(GuideRules.IsBookable(profile));
    }

    [Theory]
    [InlineData(VerificationStatus.Pending)]
    [InlineData(VerificationStatus.Rejected)]
    public void Non_approved_profile_is_not_bookable(VerificationStatus status)
    {
        var profile = new GuideProfile
        {
            VerificationStatus = status,
            CountryCode = "EG",
            City = "Cairo",
            TimeZoneId = "Africa/Cairo",
            PricePerHour = 25m,
            Languages = [new GuideLanguage()],
            Specializations = [new GuideSpecialization()]
        };

        Assert.False(GuideRules.IsBookable(profile));
    }

    [Fact]
    public void Incomplete_profile_is_not_bookable()
    {
        var profile = new GuideProfile
        {
            VerificationStatus = VerificationStatus.Approved,
            CountryCode = "EG",
            City = "Cairo",
            TimeZoneId = "Africa/Cairo",
            PricePerHour = 25m,
            Languages = [new GuideLanguage()]
        };

        Assert.False(GuideRules.IsBookable(profile));
    }
}
