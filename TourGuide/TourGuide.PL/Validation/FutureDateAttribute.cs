using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TourGuide.PL.Validation;

// Rough first filter for a DateOnly start (BR-04): the chosen day must end at least MinHoursAhead from now.
// The exact check (guide's time zone, start time) is CreateBookingValidator in BLL.
// Client side: adapter "futuredate" in wwwroot/js/validation.js.
[AttributeUsage(AttributeTargets.Property)]
public class FutureDateAttribute : ValidationAttribute, IClientModelValidator
{
    public FutureDateAttribute(int minHoursAhead)
    {
        MinHoursAhead = minHoursAhead;
        ErrorMessage = "Choose a date at least {1} hours from now.";
    }

    public int MinHoursAhead { get; }

    public override string FormatErrorMessage(string name) =>
        string.Format(ErrorMessageString, name, MinHoursAhead);

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is not DateOnly date)
            return ValidationResult.Success; // [Required] handles empty

        var time = (TimeProvider?)context.GetService(typeof(TimeProvider)) ?? TimeProvider.System;
        var endOfDayUtc = date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc).AddHours(14); // latest zone (UTC-12..+14 margin)
        return endOfDayUtc >= time.GetUtcNow().UtcDateTime.AddHours(MinHoursAhead)
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(context.DisplayName));
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        Merge(context.Attributes, "data-val", "true");
        Merge(context.Attributes, "data-val-futuredate", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));
        Merge(context.Attributes, "data-val-futuredate-hours", MinHoursAhead.ToString());
    }

    private static void Merge(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key))
            attributes.Add(key, value);
    }
}
