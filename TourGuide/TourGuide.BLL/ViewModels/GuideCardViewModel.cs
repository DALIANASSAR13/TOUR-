using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourGuide.BLL.ViewModels
{
    public class GuideCardViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string AboutMe { get; set; }
        public decimal PricePerHour { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Country { get; set; } // ? for nullable or Not Mandatory

    }
}
