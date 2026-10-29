using System.ComponentModel.DataAnnotations;
using TourGuide.DAL.Data.Enums;


namespace TourGuide.BLL.BusinessLogic
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = "";

        public UserRole Role { get; set; } = UserRole.Guide;

        public bool RememberMe { get; set; }

    }

}
