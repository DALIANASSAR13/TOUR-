using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.DAL.Data.Enums;

namespace TourGuide.DAL.Entities
{
    public class CustomTripRequest
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public CustomTripStatus Status { get; set; } = CustomTripStatus.Requested;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int TouristId { get; set; }
        public Tourist Tourist { get; set; } = null!;
        public int GuideId { get; set; }
        public Guide Guide { get; set; } = null!;
    }
}
