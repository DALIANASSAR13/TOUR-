// GuideViewModel.cs: the guide's info and the dashboard numbers


namespace TourGuide.BLL.ViewModels
{
    public class GuideViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Title { get; set; } = "";
        public string AboutMe { get; set; } = "";
        public string Country { get; set; } = "";
        public string? ProfileImageUrl { get; set; }
        public bool IsVerified { get; set; }

        public int NewRequestsCount { get; set; }
        public int ToursTodayCount { get; set; }
        public decimal TotalEarnings { get; set; }
        public int CompletedTours { get; set; }
        public double Rating { get; set; }
        public int ResponseRate { get; set; }
    }



}
