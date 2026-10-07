using ExpenseTracker.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Common.Extensions
{
    public static class QueryableExtensions
    {
        // Runs two SQL queries: one COUNT for the total, one OFFSET/FETCH for the requested page
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            int page,
            int pageSize,
            CancellationToken ct)
        {
            var totalCount = await query.CountAsync(ct);

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            return new PagedResult<T>(items, page, pageSize, totalCount);
        }
    }
}
