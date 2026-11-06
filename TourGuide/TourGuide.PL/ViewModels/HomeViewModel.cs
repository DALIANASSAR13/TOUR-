using System.Collections.Generic;

namespace TourGuide.PL.ViewModels
{
    public class HomeViewModel
    {
        public List<TourViewModel> FeaturedTours { get; set; } = new();
        public int TotalToursCount { get; set; }
    }
}
