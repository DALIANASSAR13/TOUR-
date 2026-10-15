using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourGuide.DAL.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int TourId { get; set; }
        public Tour Tour { get; set; } = null!;

        public int TouristId { get; set; }
        public Tourist Tourist { get; set; } = null!;
    }
}
