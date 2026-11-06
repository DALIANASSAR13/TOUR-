using Microsoft.AspNetCore.Mvc.ModelBinding;
using TourGuide.BLL.Common;

namespace TourGuide.PL.Extensions;

public static class ModelStateExtensions
{
    // Puts a failed Result's errors back into ModelState, so they show under the right field.
    public static void AddResultErrors(this ModelStateDictionary modelState, Result result)
    {
        foreach (var error in result.Errors)
            modelState.AddModelError(error.Field, error.Message);

        if (result.Errors.Count == 0 && !string.IsNullOrEmpty(result.Message))
            modelState.AddModelError(string.Empty, result.Message);
    }
}
