using System.Globalization;
using TourGuide.BLL.Domain;
using TourGuide.DAL.Enums;

namespace TourGuide.PL.ViewModels;

// FR-12 price box. Numbers come from the BLL quote; this class only formats them (USD).
public class PriceQuoteViewModel
{
    private static readonly CultureInfo Usd = CultureInfo.GetCultureInfo("en-US");

    public BookingType BookingType { get; set; }
    public int? Hours { get; set; }
    public int? Days { get; set; }
    public int BillableHours { get; set; }
    public decimal PricePerHour { get; set; }
    public decimal TotalPrice { get; set; }

    public string Breakdown => BookingType == BookingType.MultiDay
        ? $"{Days} days × {BookingRules.HoursPerDay} h × {PricePerHour.ToString("C", Usd)}"
        : $"{Hours} h × {PricePerHour.ToString("C", Usd)}";

    public string TotalText => TotalPrice.ToString("C", Usd);
}
