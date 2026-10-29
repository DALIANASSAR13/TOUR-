using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.BLL.ViewModels;
using TourGuide.DAL.Data.Contexts;
using TourGuide.DAL.Entities;

namespace TourGuide.BLL.BusinessLogic
{
    public class GuidesBL
    {
        public GuideListViewModel GetGuidesList(string? search)
        {
            using var context = new AppDbContext();
            var query = context.Set<Guide>().AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(g =>
                    g.FullName.Contains(search) ||
                    g.Title.Contains(search) ||
                    g.Country.Contains(search) ||
                    g.AboutMe.Contains(search));
            }

            var guides = query
                .Select(g => new GuideCardViewModel
                {
                    Id = g.Id,
                    FullName = g.FullName,
                    Country = g.Country,
                    AboutMe = g.AboutMe,
                    ProfileImageUrl = g.ProfileImageUrl,
                    PricePerHour = g.PricePerHour
                })
                .ToList();
            return new GuideListViewModel
            {
                Guides = guides,
                Search = search
            };

        }
    }
}
