namespace TourGuide.BLL.Common;

public static class ErrorCodes
{
    public const string NotFound = "not_found";
    public const string Forbidden = "forbidden";
    public const string Validation = "validation";
    public const string InvalidStatusChange = "invalid_status_change";
    public const string Overlap = "overlap";
    public const string PaymentDeadlinePassed = "payment_deadline_passed";
    public const string GuideNotBookable = "guide_not_bookable";
    public const string AlreadyReviewed = "already_reviewed";
}
