using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.DAL.Data.Enums;

namespace TourGuide.DAL.Entities
{
    public class Tour
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationDays { get; set; }
        public decimal PricePerPerson { get; set; }
        public int MaxGroupSize { get; set; }
        public int SlotsLeft { get; set; }
        public TourType Type { get; set; }
        public double AverageRating { get; set; }
        public bool IsActive { get; set; } = true;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int GuideId { get; set; }
        public Guide Guide { get; set; } = null!;

        public string Location { get; set; } = string.Empty;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
