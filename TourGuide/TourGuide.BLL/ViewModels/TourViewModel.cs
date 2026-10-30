namespace TourGuide.BLL.ViewModels
{
    public class TourViewModel
    {
        public int TourId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public decimal PricePerPerson { get; set; }
        public bool IsActive { get; set; }
        public int SlotsLeft { get; set; }
        public string Location { get; set; } = "";
        public int DurationDays { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

    }
}