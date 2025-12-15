using FluentValidation;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Validators;

/// <summary>
/// Валидатор для OrderDto
/// Проверяет корректность данных заказа при создании
/// </summary>
public class OrderDtoValidator : AbstractValidator<OrderDto>
{
    public OrderDtoValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0");

        RuleFor(x => x.RequestableId)
            .GreaterThan(0).WithMessage("Product ID must be greater than 0");

        RuleFor(x => x.RequestableType)
            .NotEmpty().WithMessage("Requestable type is required")
            .Must(type => type == "App\\Models\\Asset" || type == "App\\Models\\Accessory")
                .WithMessage("Invalid requestable type");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Quantity must not exceed 100");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative");

        RuleFor(x => x.TotalPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Total price must be non-negative");
    }
}

