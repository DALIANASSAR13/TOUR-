using TourGuide.DAL.Enums;

namespace TourGuide.BLL.Domain.Interfaces;

// FR-12, BR-11: the only place that computes a booking price.
public interface IPriceCalculator
{
    int BillableHours(BookingType type, int? hours, int? days);
    decimal Total(int billableHours, decimal pricePerHour);
}
