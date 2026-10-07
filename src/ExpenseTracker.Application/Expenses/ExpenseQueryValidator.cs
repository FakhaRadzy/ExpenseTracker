using FluentValidation;

namespace ExpenseTracker.Application.Expenses
{
    public class ExpenseQueryValidator : AbstractValidator<ExpenseQuery>
    {
        public const int MaxPageSize = 100;

        public ExpenseQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize).InclusiveBetween(1, MaxPageSize);

            RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From).When(x => x.From is not null && x.To is not null).WithMessage("'To' must be on or after 'From;");

            RuleFor(x => x.MinAmount).GreaterThanOrEqualTo(0).When(x => x.MinAmount is not null);

            RuleFor(x => x.MaxAmount).GreaterThanOrEqualTo(x => x.MinAmount).When(x => x.MinAmount is not null && x.MaxAmount is not null).WithMessage("'Max Amount' must be greater than or equal to 'Min Amount'.");

            RuleFor(x => x.Search).MaximumLength(100);

            RuleFor(x => x.SortBy).IsInEnum();
            RuleFor(x => x.SortDirection).IsInEnum();
        }
    }
}
