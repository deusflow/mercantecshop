using FluentValidation;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Validators;

/// <summary>
/// Валидатор для AccessoryDto
/// Проверяет корректность данных аксессуара при создании/обновлении
/// </summary>
public class AccessoryDtoValidator : AbstractValidator<AccessoryDto>
{
    public AccessoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Accessory name is required")
            .MinimumLength(2).WithMessage("Accessory name must be at least 2 characters")
            .MaximumLength(200).WithMessage("Accessory name must not exceed 200 characters");

        RuleFor(x => x.Qty)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be non-negative");

        RuleFor(x => x.MinAmt)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum amount must be non-negative")
            .When(x => x.MinAmt.HasValue);

        RuleFor(x => x.PurchaseCost)
            .GreaterThanOrEqualTo(0).WithMessage("Purchase cost must be non-negative")
            .When(x => x.PurchaseCost.HasValue);

        RuleFor(x => x.ModelNumber)
            .MaximumLength(100).WithMessage("Model number must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.ModelNumber));

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

