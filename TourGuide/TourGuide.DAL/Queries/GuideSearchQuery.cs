namespace TourGuide.DAL.Queries;

// FR-10 search criteria passed from BLL to IGuideProfileRepository.SearchAsync.
// "Approved and profile complete" (BR-02) is not a field: the repository always adds it.
public class GuideSearchQuery
{
    public string? CountryCode { get; set; }
    public int? LanguageId { get; set; }
    public int? SpecializationId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; }
}
