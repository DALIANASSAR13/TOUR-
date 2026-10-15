using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.DAL.Data.Enums;

namespace TourGuide.DAL.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public decimal GuideFee { get; set; }
        public decimal PlatformFee { get; set; }
        public DateTime? PaidAt { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
    }
}
