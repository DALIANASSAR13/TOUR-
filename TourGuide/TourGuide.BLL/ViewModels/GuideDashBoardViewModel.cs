// GuideDashboardViewModel.cs: the parent the view receives

namespace TourGuide.BLL.ViewModels
{


    public class GuideDashboardViewModel
    {
        public GuideViewModel Guide { get; set; } = new();
        public List<BookingRequestViewModel> Requests { get; set; } = new();
        public List<TourViewModel> Tours { get; set; } = new();
    }

}