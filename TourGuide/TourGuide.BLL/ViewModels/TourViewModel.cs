// TourViewModel.cs: one tour card (lean, matches the new UI)


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
    }

}
