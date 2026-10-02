using ExpenseTracker.Application.Expenses;

namespace ExpenseTracker.Application.Expenses
{
    public interface IExpenseService
    {
        Task<IReadOnlyList<ExpenseResponse>> GetAllAsync(CancellationToken ct);
        Task<ExpenseResponse?> GetIdAsync(Guid id, CancellationToken ct);
        Task<ExpenseResponse> CreateAsync(ExpenseRequest request, CancellationToken ct);
        Task<ExpenseResponse?> UpdateAsync(Guid id, ExpenseRequest request, CancellationToken ct);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    }
}
