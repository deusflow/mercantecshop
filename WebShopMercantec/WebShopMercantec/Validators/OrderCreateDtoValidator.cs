using FluentValidation;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Validators;

public class OrderCreateDtoValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateDtoValidator()
    {
        RuleFor(x => x.RequestableId)
            .GreaterThan(0).WithMessage("Product ID must be greater than 0");

        RuleFor(x => x.RequestableType)
            .NotEmpty().WithMessage("Requestable type is required")
            .Must(t => t == "asset" || t == "accessory")
            .WithMessage("Type must be 'asset' or 'accessory'");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1")
            .LessThanOrEqualTo(100).WithMessage("Quantity cannot exceed 100");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters")
            .When(x => x.Notes != null);
    }
}

