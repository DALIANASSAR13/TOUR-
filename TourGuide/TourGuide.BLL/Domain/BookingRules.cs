namespace TourGuide.BLL.Domain;

// Every booking limit lives here only. ViewModel attributes and validators read these constants.
public static class BookingRules
{
    public const int MinHours = 2;            // BR-04
    public const int MaxHours = 10;           // BR-04
    public const int MinDays = 2;             // BR-04
    public const int MaxDays = 14;            // BR-04
    public const int HoursPerDay = 8;         // BR-11
    public const int MinAdvanceHours = 24;    // BR-04
    public const int PendingExpiryHours = 24; // BR-06
    public const int PaymentWindowHours = 24; // BR-08

    public const double MaxPricePerHour = 1000;   // Class Specs decision 8
    public const int MaxYearsOfExperience = 60;   // Class Specs decision 8
    public const int MinRating = 1;
    public const int MaxRating = 5;
    public const int MaxCommentLength = 1000;
    public const int MinRejectionReasonLength = 10;
    public const int MaxRejectionReasonLength = 500;
}
