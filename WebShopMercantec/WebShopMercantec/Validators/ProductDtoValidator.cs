using FluentValidation;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Validators;

/// <summary>
/// Валидатор для ProductDto
/// Проверяет корректность данных продукта при создании/обновлении
/// </summary>
public class ProductDtoValidator : AbstractValidator<ProductDto>
{
    public ProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MinimumLength(2).WithMessage("Product name must be at least 2 characters")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");

        RuleFor(x => x.AssetTag)
            .NotEmpty().WithMessage("Asset tag is required")
            .MaximumLength(50).WithMessage("Asset tag must not exceed 50 characters");

        RuleFor(x => x.PurchaseCost)
            .GreaterThanOrEqualTo(0).WithMessage("Purchase cost must be non-negative")
            .When(x => x.PurchaseCost.HasValue);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative");

        RuleFor(x => x.Serial)
            .MaximumLength(100).WithMessage("Serial number must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Serial));

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

