using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using TourGuide.BLL.Domain;

namespace TourGuide.PL.ViewModels;

// FR-10 search page (UI screen 5 + filter sidebar of screen 8). The filters come from the query string (GET form);
// the rest is filled by the controller through AutoMapper.
public class SearchViewModel
{
    // ---------- filters (input) ----------
    [Display(Name = "Country")]
    [StringLength(2, ErrorMessage = "Choose a country from the list.")]
    public string? CountryCode { get; set; }

    [Display(Name = "Language")]
    public int? LanguageId { get; set; }

    [Display(Name = "Specialization")]
    public int? SpecializationId { get; set; }

    [Display(Name = "Min price")]
    [Range(0, BookingRules.MaxPricePerHour, ErrorMessage = "Enter a price from {1} to {2}.")]
    public decimal? MinPrice { get; set; }

    [Display(Name = "Max price")]
    [Range(0, BookingRules.MaxPricePerHour, ErrorMessage = "Enter a price from {1} to {2}.")]
    public decimal? MaxPrice { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Choose page 1 or later.")]
    public int Page { get; set; } = 1;

    // ---------- dropdown choices ----------
    [BindNever] public List<SelectListItem> Countries { get; set; } = new();
    [BindNever] public List<SelectListItem> Languages { get; set; } = new();
    [BindNever] public List<SelectListItem> Specializations { get; set; } = new();

    // ---------- results ----------
    [BindNever] public List<GuideCardViewModel> Guides { get; set; } = new();
    [BindNever] public int PageSize { get; set; }
    [BindNever] public int TotalCount { get; set; }
    [BindNever] public int TotalPages { get; set; }

    public bool HasFilters =>
        !string.IsNullOrWhiteSpace(CountryCode) || LanguageId.HasValue || SpecializationId.HasValue
        || MinPrice.HasValue || MaxPrice.HasValue;

    // Route values for a pager link that keeps the current filters.
    public object RouteFor(int page) => new { CountryCode, LanguageId, SpecializationId, MinPrice, MaxPrice, page };
}
