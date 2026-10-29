// BookingRequestViewModel.cs: one row in "Incoming Booking Requests"

namespace TourGuide.BLL.ViewModels
{
    public class BookingRequestViewModel
    {
        public int BookingId { get; set; }
        public string TouristName { get; set; } = "";
        public string? TouristImageUrl { get; set; }
        public string TourTitle { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
    }

}


