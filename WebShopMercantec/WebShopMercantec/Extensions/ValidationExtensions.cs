using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace WebShopMercantec.Extensions;

public static class ValidationExtensions
{
    
    public static async Task<ActionResult?> ValidateAsync<T>(
        this ControllerBase controller,
        T model,
        IValidator<T> validator)
    {
        var validationResult = await validator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return controller.BadRequest(new
            {
                message = "Validation failed",
                errors = errors
            });
        }

        return null; // Validation passed
    }
}
