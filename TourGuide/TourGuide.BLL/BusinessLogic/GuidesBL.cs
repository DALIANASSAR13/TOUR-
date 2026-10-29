using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.BLL.ViewModels;
using TourGuide.DAL.Data.Contexts;
using TourGuide.DAL.Data.Enums;
using TourGuide.DAL.Entities;

namespace TourGuide.BLL.BusinessLogic
{
    public class GuidesBL
    {
        public AppDbContext context;

        public GuidesBL()
        {
            context = new AppDbContext();
        }
        public GuideListViewModel GetGuidesList(string? search)
        {
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


        public GuideDashboardViewModel? GetDashboard(int guideId)
        {
            using var context = new AppDbContext();

            // 1. the guide (null if the id doesn't exist)
            var guide = context.Set<Guide>().FirstOrDefault(g => g.Id == guideId);
            if (guide == null)
                return null;

            // 2. every booking on this guide's tours (a query, not executed yet)
            var bookings = context.Set<Booking>()
                .Where(b => b.Tour.GuideId == guideId);

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // 3. the numbers
            var newRequests = bookings.Count(b => b.Status == BookingStatus.Pending);

            var toursToday = bookings.Count(b =>
                b.Status == BookingStatus.Pending &&
                b.Tour.StartDate >= today && b.Tour.StartDate < tomorrow);

            var earnings = bookings
                .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
                .Sum(b => (decimal?)b.TotalPrice) ?? 0;

            var rating = context.Set<Tour>()
                .Where(t => t.GuideId == guideId)
                .Average(t => (double?)t.AverageRating) ?? 0;

            var totalBookings = bookings.Count();
            var answered = bookings.Count(b => b.Status != BookingStatus.Pending);
            var responseRate = totalBookings == 0 ? 0 : answered * 100 / totalBookings;

            // 4. pending request rows
            var requests = bookings
                .Where(b => b.Status == BookingStatus.Pending)
                .OrderBy(b => b.Tour.StartDate)
                .Select(b => new BookingRequestViewModel
                {
                    BookingId = b.Id,
                    TouristName = b.Tourist.FullName,
                    TouristImageUrl = b.Tourist.ProfileImageUrl,
                    TourTitle = b.Tour.Title,
                    StartDate = b.Tour.StartDate,
                    EndDate = b.Tour.EndDate,
                    TotalPrice = b.TotalPrice
                })
                .ToList();

            // 5. tour cards
            var tours = context.Set<Tour>()
                .Where(t => t.GuideId == guideId)
                .Select(t => new TourViewModel
                {
                    TourId = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    ImageUrl = t.ImageUrl,
                    PricePerPerson = t.PricePerPerson,
                    IsActive = t.IsActive,
                    SlotsLeft = t.SlotsLeft
                })
                .ToList();

            // 6. pack the three parts into the parent ViewModel
            return new GuideDashboardViewModel
            {
                Guide = new GuideViewModel
                {
                    Id = guide.Id,
                    FullName = guide.FullName,
                    Title = guide.Title,
                    AboutMe = guide.AboutMe,
                    Country = guide.Country,
                    ProfileImageUrl = guide.ProfileImageUrl,
                    IsVerified = guide.IsVerified,
                    NewRequestsCount = newRequests,
                    ToursTodayCount = toursToday,
                    TotalEarnings = earnings,
                    CompletedTours = guide.ToursCompleted,
                    Rating = rating,
                    ResponseRate = responseRate
                },
                Requests = requests,
                Tours = tours
            };
        }



    }
}
