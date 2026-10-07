using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Expenses
{
    public enum ExpenseSortField
    {
        Date,
        Amount
    }

    // Every property is optional. Anything left out means "don't filter on this".
    public record ExpenseQuery
    {
        public DateOnly? From { get; init; }
        public DateOnly? To { get; init; }
        public Guid? CategoryId { get; init; }
        public PaymentMethod? PaymentMethod { get; init; }
        public decimal? MinAmount { get; init; }
        public decimal? MaxAmount { get; init; }
        public string? Search { get; init; }

        public ExpenseSortField SortBy { get; init; } = ExpenseSortField.Date;
        public SortDirection SortDirection { get; init; } = SortDirection.Desc;

        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }
}
