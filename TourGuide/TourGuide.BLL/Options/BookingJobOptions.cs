namespace TourGuide.BLL.Options;

// Bound from appsettings.json "BookingJob". How often the scheduled job runs (NFR-06).
public class BookingJobOptions
{
    public const string SectionName = "BookingJob";

    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(5);
}
