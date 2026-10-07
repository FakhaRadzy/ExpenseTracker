using System.Linq.Expressions;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using ExpenseTracker.Application.Common.Models;
using ExpenseTracker.Application.Common.Extensions;

namespace ExpenseTracker.Application.Expenses
{
    public class ExpenseService(IApplicationDbContext db, ICurrentUserService currentUser, IValidator<ExpenseRequest> validator, IValidator<ExpenseQuery> queryValidator) : IExpenseService
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

        public async Task<PagedResult<ExpenseResponse>> GetAllAsync(ExpenseQuery query, CancellationToken ct)
        {
            await queryValidator.ValidateAndThrowAsync(query, ct);

            var expenses = UserExpenses();

            // ── Filters: each one is only added if the client asked for it ──
            if (query.From is not null)
            {
                expenses = expenses.Where(e => e.Date >= query.From.Value);
            }

            if (query.To is not null)
            {
                expenses = expenses.Where(e => e.Date <= query.To.Value);
            }

            if (query.CategoryId is not null)
            {
                expenses = expenses.Where(e => e.CategoryId == query.CategoryId.Value);
            }

            if (query.PaymentMethod is not null)
            {
                expenses = expenses.Where(e => e.PaymentMethod == query.PaymentMethod.Value);
            }

            if (query.MinAmount is not null)
            {
                expenses = expenses.Where(e => e.Amount >= query.MinAmount.Value);
            }

            if (query.MaxAmount is not null)
            {
                expenses = expenses.Where(e => e.Amount <= query.MaxAmount.Value);
            }

            if (!string.IsNullOrEmpty(query.Search))
            {
                var search = query.Search.Trim(); 
                expenses = expenses.Where(e => e.Description.Contains(search) || (e.Notes != null && e.Notes.Contains(search)));
            }

            // ── Sorting: always end with a unique tie-breaker so paging is stable
            expenses = (query.SortBy, query.SortDirection) switch
            {
                (ExpenseSortField.Amount, SortDirection.Asc) => expenses.OrderBy(e => e.Amount).ThenBy(e => e.Id),
                (ExpenseSortField.Amount, SortDirection.Desc) => expenses.OrderByDescending(e => e.Amount).ThenBy(e => e.Id),
                (ExpenseSortField.Date, SortDirection.Asc) => expenses.OrderBy(e => e.Date).ThenBy(e => e.Id),
                _ => expenses.OrderByDescending(e => e.Date).ThenBy(e => e.Id)
            };

            // ── Project to DTOs, then fetch just the requested page ──
            return await expenses.Select(ToResponse).ToPagedResultAsync(query.Page, query.PageSize, ct);

        }

        public async Task<ExpenseResponse?> GetIdAsync (Guid id, CancellationToken ct)
        {
            return await UserExpenses().Where(e => e.Id == id).Select(ToResponse).FirstOrDefaultAsync(ct);
        }

        public async Task<ExpenseResponse> CreateAsync (ExpenseRequest request, CancellationToken ct)
        {
            await validator.ValidateAndThrowAsync(request, ct);

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
            await validator.ValidateAndThrowAsync(request, ct);

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
