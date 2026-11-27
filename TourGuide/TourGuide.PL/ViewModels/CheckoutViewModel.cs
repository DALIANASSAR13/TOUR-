namespace TourGuide.PL.ViewModels;

// Payment page (UI screen 4), FR-14. The booking fields and their formatting come from BookingItemViewModel,
// so the checkout shows the same texts as the card on My Bookings. No card fields here (NFR-04).
public class CheckoutViewModel : BookingItemViewModel
{
    // "Nov 12, 18:30 (GMT+2)" — the guide's local time.
    public string PayBeforeText => DeadlineText;
}
