using System.Collections.Generic;

namespace TourGuide.BLL.ViewModels
{
    public class HomeViewModel
    {
        public List<TourViewModel> FeaturedTours { get; set; } = new();
        public int TotalToursCount { get; set; }
    }
}
