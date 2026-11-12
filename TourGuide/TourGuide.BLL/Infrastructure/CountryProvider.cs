using System.Globalization;
using TourGuide.BLL.DTOs.Search;

namespace TourGuide.BLL.Infrastructure;

// ISO 3166-1 alpha-2 countries with English names, built once from .NET's region data. Stateless (Singleton).
public class CountryProvider : ICountryProvider
{
    private static readonly List<CountryDto> Countries = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
        .Select(TryRegion)
        .OfType<RegionInfo>()
        .Where(r => r.TwoLetterISORegionName.Length == 2 && r.TwoLetterISORegionName.All(char.IsLetter))
        .GroupBy(r => r.TwoLetterISORegionName.ToUpperInvariant())
        .Select(g => new CountryDto { Code = g.Key, Name = g.First().EnglishName })
        .OrderBy(c => c.Name, StringComparer.Ordinal)
        .ToList();

    private static readonly HashSet<string> Codes =
        new(Countries.Select(c => c.Code), StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, string> Names =
        Countries.ToDictionary(c => c.Code, c => c.Name, StringComparer.OrdinalIgnoreCase);

    // A copy, so callers cannot change the shared list.
    public List<CountryDto> GetAll() =>
        Countries.Select(c => new CountryDto { Code = c.Code, Name = c.Name }).ToList();

    public bool Exists(string code) => !string.IsNullOrWhiteSpace(code) && Codes.Contains(code.Trim());

    public string? GetName(string? code) =>
        string.IsNullOrWhiteSpace(code) ? null : Names.TryGetValue(code.Trim(), out var name) ? name : code;

    private static RegionInfo? TryRegion(CultureInfo culture)
    {
        try { return new RegionInfo(culture.Name); }
        catch (ArgumentException) { return null; }
    }
}
