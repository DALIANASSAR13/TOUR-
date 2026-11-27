using Microsoft.AspNetCore.Identity;

namespace TourGuide.DAL.Entities;

// One account class for Tourist, Guide and Admin. The role is an Identity role (Constants/Roles).
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }

    public GuideProfile? GuideProfile { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
