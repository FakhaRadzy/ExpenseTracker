using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Application.Expenses
{
    public record ExpenseRequest(
        Guid CategoryId,
        decimal Amount,
        string Description,
        DateOnly Date,
        PaymentMethod PaymentMethod,
        string? Notes);

    public record ExpenseResponse(
        Guid Id,
        decimal Amount,
        string Description,
        DateOnly Date,
        PaymentMethod PaymentMethod,
        string? Notes,
        Guid CategoryId,
        string CategoryName,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc);
    
}
