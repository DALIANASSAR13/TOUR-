namespace TourGuide.BLL.DTOs.Search;

// FR-10 request: every filter is optional. Checked by GuideSearchFilterValidator.
public class GuideSearchFilter
{
    public string? CountryCode { get; set; }
    public int? LanguageId { get; set; }
    public int? SpecializationId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int Page { get; set; } = 1;
}
