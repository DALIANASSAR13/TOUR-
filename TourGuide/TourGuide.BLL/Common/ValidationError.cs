namespace TourGuide.BLL.Common;

// One field error. Field matches the request DTO property name, so the PL can show it under the right input.
public record ValidationError(string Field, string Message);
