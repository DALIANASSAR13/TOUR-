using TourGuide.DAL.Data;
using TourGuide.DAL.Data.Contexts;
using TourGuide.DAL.Data.Enums;
using TourGuide.DAL.Entities;

namespace TourGuide.BLL.BusinessLogic
{
    public class AccountBL
    {

        AppDbContext context = new AppDbContext();
        public AccountBL()
        {
            context = new AppDbContext();
        }
        public int? FindUserId(string email, UserRole role)
        {

            return context.Set<ApplicationUser>()
                .Where(u => u.Email == email && u.Role == role)
                .Select(u => (int?)u.Id)
                .FirstOrDefault();
        }

    }
}
