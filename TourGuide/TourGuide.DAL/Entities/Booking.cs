using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.DAL.Data.Enums;

namespace TourGuide.DAL.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int NumberOfPeople { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int TourId { get; set; }
        public Tour Tour { get; set; } = null!;

        public int TouristId { get; set; }
        public Tourist Tourist { get; set; } = null!;

        public Payment? Payment { get; set; }
    }
}
