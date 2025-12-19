using FluentValidation;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Validators;

/// <summary>
/// Валидатор для CategoryDto
/// Проверяет корректность данных категории при создании/обновлении
/// </summary>
public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .MinimumLength(2).WithMessage("Category name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters");

        RuleFor(x => x.CategoryType)
            .Must(BeValidCategoryType)
            .WithMessage("Category type must be one of: asset, accessory, consumable, component, license")
            .When(x => !string.IsNullOrEmpty(x.CategoryType));
    }

    private bool BeValidCategoryType(string? categoryType)
    {
        if (string.IsNullOrEmpty(categoryType))
            return true;

        var validTypes = new[] { "asset", "accessory", "consumable", "component", "license" };
        return validTypes.Contains(categoryType.ToLower());
    }
}

