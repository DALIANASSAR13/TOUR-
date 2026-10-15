using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourGuide.DAL.Data.Enums
{
    public enum PaymentStatus
    {
      Pending, 
      HeldInEscrow, 
      Paid, 
      Refunded,
      Failed 
    }
}
