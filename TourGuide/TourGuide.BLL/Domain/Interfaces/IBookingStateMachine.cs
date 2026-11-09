using TourGuide.BLL.Common;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Enums;

namespace TourGuide.BLL.Domain.Interfaces;

// NFR-05, BR-05 – BR-10: the only code that changes Booking.Status after creation.
public interface IBookingStateMachine
{
    bool CanChange(BookingStatus from, BookingStatus to, Actor actor);
    Result Apply(Booking booking, BookingStatus to, Actor actor, DateTime nowUtc, CancellationReason? reason = null);
}
