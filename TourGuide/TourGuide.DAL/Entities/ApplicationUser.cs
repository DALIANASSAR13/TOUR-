using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.DAL.Data.Enums;

namespace TourGuide.DAL.Entities
{

    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? GoogleId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Country { get; set; } = string.Empty;
    }

}
