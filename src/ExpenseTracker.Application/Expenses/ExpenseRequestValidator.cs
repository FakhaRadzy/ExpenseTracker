using ExpenseTracker.Domain.Entities;
using FluentValidation;

namespace ExpenseTracker.Application.Expenses
{
    public class ExpenseRequestValidator : AbstractValidator<ExpenseRequest>
    {
        public ExpenseRequestValidator(TimeProvider timeProvider)
        {
            RuleFor(x => x.CategoryId).NotEmpty();

            RuleFor(x => x.Amount).GreaterThan(0).PrecisionScale(18, 2, ignoreTrailingZeros: true).WithMessage("Amount cannot have more than 2 decimal places.");

            RuleFor(x => x.Description).NotEmpty().MaximumLength(Expense.DescriptionMaxLength);

            RuleFor(x => x.Date).NotEmpty()
            // +1 day of slack so users in timezones ahead of UTC (like Malaysia, UTC+8) aren't rejected
            .LessThanOrEqualTo(_ => DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime.AddDays(1))).WithMessage("Date cannot be in the future.");

            RuleFor(x => x.PaymentMethod).IsInEnum();

            RuleFor(x => x.Notes).MaximumLength(Expense.NotesMaxLength);
        }
    }
}
