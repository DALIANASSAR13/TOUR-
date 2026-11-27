using TourGuide.DAL.Entities;
using TourGuide.DAL.Enums;

namespace TourGuide.BLL.Domain;

// BR-02: a guide is bookable only when Approved and the profile is complete (Class Specs §2).
// Needs Languages and Specializations loaded.
public static class GuideRules
{
    public static bool IsBookable(GuideProfile g) =>
        g.VerificationStatus == VerificationStatus.Approved
        && !string.IsNullOrWhiteSpace(g.CountryCode)
        && !string.IsNullOrWhiteSpace(g.City)
        && !string.IsNullOrWhiteSpace(g.TimeZoneId)
        && g.PricePerHour is > 0
        && g.Languages.Count > 0
        && g.Specializations.Count > 0;
}
