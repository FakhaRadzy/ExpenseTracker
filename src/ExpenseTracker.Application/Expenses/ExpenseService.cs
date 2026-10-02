using System.Linq.Expressions;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Expenses
{
    public class ExpenseService(IApplicationDbContext db, ICurrentUserService currentUser) : IExpenseService
    {
        // One mapping, reused by every query. EF turns it into SQL (including the JOIN for CategoryName).
        private static readonly Expression<Func<Expense, ExpenseResponse>> ToResponse = e => new ExpenseResponse(
            e.Id,
            e.Amount,
            e.Description,
            e.Date,
            e.PaymentMethod,
            e.Notes,
            e.CategoryId,
            e.Category.Name,
            e.CreatedAtUtc,
            e.UpdatedAtUtc
            );

        public async Task<IReadOnlyList<ExpenseResponse>> GetAllAsync(CancellationToken ct)
        {
            return await UserExpenses().OrderByDescending(e => e.Date).ThenByDescending(e => e.CreatedAtUtc).Select(ToResponse).ToListAsync(ct);
        }

        public async Task<ExpenseResponse?> GetIdAsync (Guid id, CancellationToken ct)
        {
            return await UserExpenses().Where(e => e.Id == id).Select(ToResponse).FirstOrDefaultAsync();
        }

        public async Task<ExpenseResponse> CreateAsync (ExpenseRequest request, CancellationToken ct)
        {
            await EnsureCategoryExistsAsync(request.CategoryId, ct);

            var expense = new Expense(
                currentUser.UserId,
                request.CategoryId,
                request.Amount,
                request.Description,
                request.Date,
                request.PaymentMethod,
                request.Notes);

            db.Expenses.Add(expense);
            await db.SaveChangesAsync(ct);

            return (await GetIdAsync(expense.Id, ct))!;
        }

        public async Task<ExpenseResponse?> UpdateAsync(Guid id, ExpenseRequest request, CancellationToken ct)
        {
            var expense = await UserExpenses().FirstOrDefaultAsync(e => e.Id == id, ct);

            if (expense is null) return null;

            if (expense.CategoryId != request.CategoryId)
            {
                await EnsureCategoryExistsAsync(request.CategoryId, ct);
            }

            expense.Update(
                request.CategoryId,
                request.Amount,
                request.Description,
                request.Date,
                request.PaymentMethod,
                request.Notes);

            await db.SaveChangesAsync(ct);

            return await GetIdAsync(id, ct);
        }

        public async Task<bool> DeleteAsync (Guid id, CancellationToken ct)
        {
            var deleteCount = await UserExpenses().Where(e => e.Id == id).ExecuteDeleteAsync(ct);

            return deleteCount > 0;
        }

        // Every query starts here, so we can never forget the user filter
        private IQueryable<Expense> UserExpenses() => db.Expenses.Where(e => e.UserId == currentUser.UserId);

        private async Task EnsureCategoryExistsAsync(Guid categoryId, CancellationToken ct)
        {
            var exists = await db.Categories.AnyAsync(c => c.Id == categoryId && c.UserId == currentUser.UserId, ct);

            if (!exists)
            {
                throw new DomainException("Category not found.");
            }
        }
    }
}
