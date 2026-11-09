using TourGuide.BLL.Domain.Interfaces;
using TourGuide.DAL.Enums;

namespace TourGuide.BLL.Domain;

// BR-11: Hourly -> Hours; MultiDay -> Days x HoursPerDay. Total = billable hours x price per hour.
// Inputs are validated before this is called, so a missing value is a programming error.
public class PriceCalculator : IPriceCalculator
{
    public int BillableHours(BookingType type, int? hours, int? days) => type switch
    {
        BookingType.Hourly => hours ?? throw new ArgumentNullException(nameof(hours)),
        BookingType.MultiDay => (days ?? throw new ArgumentNullException(nameof(days))) * BookingRules.HoursPerDay,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    public decimal Total(int billableHours, decimal pricePerHour) =>
        Math.Round(billableHours * pricePerHour, 2, MidpointRounding.AwayFromZero);
}
