using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourGuide.DAL.Entities
{
    public class Guide : ApplicationUser
    {
        public string Title { get; set; } = string.Empty;
        public string AboutMe { get; set; } = string.Empty;
        public int ToursCompleted { get; set; }
        public bool IsVerified { get; set; }
        public decimal PricePerHour { get; set; }

        public ICollection<Tour> Tours { get; set; } = new List<Tour>();
        public ICollection<CustomTripRequest> CustomTripRequests { get; set; } = new List<CustomTripRequest>();


    }
}
