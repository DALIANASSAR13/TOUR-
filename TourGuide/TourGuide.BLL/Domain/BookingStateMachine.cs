using TourGuide.BLL.Common;
using TourGuide.BLL.Domain.Interfaces;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Enums;

namespace TourGuide.BLL.Domain;

// The allowed status changes from Class Specs §5. Anything not in the table is refused.
// Data conditions (overlap, payment deadline, 24 h without answer, EndUtc passed) are checked by the
// calling services (2.7 – 3.1) before they call Apply.
public class BookingStateMachine : IBookingStateMachine
{
    private sealed record Transition(BookingStatus From, BookingStatus To, Actor Actor, CancellationReason? Reason);

    private static readonly Transition[] Table =
    {
        new(BookingStatus.Pending,   BookingStatus.Accepted,  Actor.Guide,   null),
        new(BookingStatus.Pending,   BookingStatus.Rejected,  Actor.Guide,   null),
        new(BookingStatus.Pending,   BookingStatus.Cancelled, Actor.Tourist, CancellationReason.WithdrawnByTourist),
        new(BookingStatus.Pending,   BookingStatus.Cancelled, Actor.System,  CancellationReason.NoGuideResponse),
        new(BookingStatus.Accepted,  BookingStatus.Confirmed, Actor.Tourist, null),
        new(BookingStatus.Accepted,  BookingStatus.Cancelled, Actor.Tourist, CancellationReason.WithdrawnByTourist),
        new(BookingStatus.Accepted,  BookingStatus.Cancelled, Actor.System,  CancellationReason.NotPaid),
        new(BookingStatus.Confirmed, BookingStatus.Completed, Actor.System,  null),
    };

    public bool CanChange(BookingStatus from, BookingStatus to, Actor actor) =>
        Table.Any(t => t.From == from && t.To == to && t.Actor == actor);

    public Result Apply(Booking booking, BookingStatus to, Actor actor, DateTime nowUtc, CancellationReason? reason = null)
    {
        var row = Table.FirstOrDefault(t => t.From == booking.Status && t.To == to && t.Actor == actor && t.Reason == reason);
        if (row is null)
            return Result.Fail(ErrorCodes.InvalidStatusChange,
                $"A {booking.Status.ToString().ToLowerInvariant()} booking cannot be changed to {to.ToString().ToLowerInvariant()}.");

        booking.Status = to;
        switch (to)
        {
            case BookingStatus.Accepted:
                booking.RespondedAtUtc = nowUtc;
                var windowEnd = nowUtc.AddHours(BookingRules.PaymentWindowHours);           // BR-08
                booking.PaymentDeadlineUtc = windowEnd < booking.StartUtc ? windowEnd : booking.StartUtc;
                break;
            case BookingStatus.Rejected:
                booking.RespondedAtUtc = nowUtc;
                break;
            case BookingStatus.Cancelled:
                booking.CancelledAtUtc = nowUtc;
                booking.CancellationReason = reason;
                break;
            case BookingStatus.Completed:
                booking.CompletedAtUtc = nowUtc;
                break;
        }

        return Result.Ok();
    }
}
