using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace WebShopMercantec.Extensions;

/// <summary>
/// Extension методы для валидации в контроллерах
/// Позволяют легко валидировать DTOs и автоматически возвращать ошибки
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Валидирует объект и возвращает BadRequest с ошибками если валидация не прошла
    /// 
    /// ИСПОЛЬЗОВАНИЕ В КОНТРОЛЛЕРЕ:
    /// var validationResult = await this.ValidateAsync(createDto, _validator);
    /// if (validationResult != null) return validationResult;
    /// </summary>
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

        return null; // Валидация прошла успешно
    }
}

