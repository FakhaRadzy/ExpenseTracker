using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Category> Categories { get; }
        DbSet<Expense> Expenses { get; }
        DbSet<Budget> Budgets { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
