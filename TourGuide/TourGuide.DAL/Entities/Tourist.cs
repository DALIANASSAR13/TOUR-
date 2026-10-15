using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourGuide.DAL.Entities
{
    public class Tourist : ApplicationUser
    {
        public ICollection<Booking> TripHistory { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
