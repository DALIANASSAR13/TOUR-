using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Options;
using TourGuide.BLL.Options;

namespace TourGuide.PL.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class MaxFileSizeAttribute : ValidationAttribute, IClientModelValidator
{
    public MaxFileSizeAttribute() => ErrorMessage = "The file is larger than {0} MB. Choose a smaller file.";

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value is not IFormFile file)
            return ValidationResult.Success; // [Required] handles empty

        var max = ReadLimit(context);
        return file.Length <= max
            ? ValidationResult.Success
            : new ValidationResult(string.Format(ErrorMessageString, max / 1024 / 1024));
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        var max = ReadLimit(context.ActionContext.HttpContext.RequestServices);
        Merge(context.Attributes, "data-val", "true");
        Merge(context.Attributes, "data-val-maxfilesize", string.Format(ErrorMessageString, max / 1024 / 1024));
        Merge(context.Attributes, "data-val-maxfilesize-bytes", max.ToString());
    }

    private static long ReadLimit(IServiceProvider services) =>
        (services.GetService(typeof(IOptions<FileStorageOptions>)) as IOptions<FileStorageOptions>)?.Value.MaxFileSizeBytes
        ?? new FileStorageOptions().MaxFileSizeBytes;

    private static void Merge(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key)) attributes.Add(key, value);
    }
}
