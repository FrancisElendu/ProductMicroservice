using FluentValidation;
using Product.Application.Commands;

namespace Product.Application.Validators
{
    // This validator will automatically be called by ValidationBehavior
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero")
                .LessThan(1000000).WithMessage("Price must be less than 1,000,000");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative")
                .LessThan(10000).WithMessage("Stock quantity must be less than 10,000");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required")
                .MaximumLength(100).WithMessage("Category must not exceed 100 characters");

            RuleFor(x => x.Sku)
                .NotEmpty().WithMessage("SKU is required")
                .MaximumLength(50).WithMessage("SKU must not exceed 50 characters")
                .Matches("^[A-Z0-9-]+$").WithMessage("SKU can only contain uppercase letters, numbers, and hyphens");
        }
    }
}
