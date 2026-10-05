using ExpenseTracker.Domain.Entities;
using FluentValidation;

namespace ExpenseTracker.Application.Categories
{
    public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
    {
        public CategoryRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(Category.NameMaxLength);

            RuleFor(x => x.Color).Matches("^#[0-9A-Fa-f]{6}$").WithMessage("Color must be a hex value like #FF5733.");
        }
    }
}
