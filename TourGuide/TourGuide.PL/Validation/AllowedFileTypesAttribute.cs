using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TourGuide.PL.Validation;

// First filter on the file name. The real check is the content signature in BLL.
[AttributeUsage(AttributeTargets.Property)]
public class AllowedFileTypesAttribute : ValidationAttribute, IClientModelValidator
{
    private static readonly string[] Extensions = { ".pdf", ".jpg", ".jpeg", ".png" };

    public AllowedFileTypesAttribute() => ErrorMessage = "Upload a PDF, JPG or PNG file.";

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is not IFormFile file)
            return ValidationResult.Success; // [Required] handles empty

        return Extensions.Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase)
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage);
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        Merge(context.Attributes, "data-val", "true");
        Merge(context.Attributes, "data-val-allowedfiletypes", ErrorMessage!);
        Merge(context.Attributes, "data-val-allowedfiletypes-extensions", string.Join(",", Extensions));
    }

    private static void Merge(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key)) attributes.Add(key, value);
    }
}
