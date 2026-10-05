using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Application.Common.Exceptions;
using FluentValidation;

namespace ExpenseTracker.Application.Categories
{
    public class CategoryService(IApplicationDbContext db, ICurrentUserService currentUser, IValidator<CategoryRequest> validator) : ICategoryService
    {
        public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken ct)
        {
            return await db.Categories
                .Where(c => c.UserId == currentUser.UserId)
                .OrderBy(c => c.Name)
                .Select(c => new CategoryResponse(c.Id, c.Name, c.Color))
                .ToListAsync(ct);
        }

        public async Task<CategoryResponse?> GetIdAsync(Guid id, CancellationToken ct)
        {
            return await db.Categories
                .Where(c => c.Id == id && c.UserId == currentUser.UserId)
                .Select(c => new CategoryResponse(c.Id, c.Name, c.Color))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<CategoryResponse> CreateAsync(CategoryRequest request, CancellationToken ct)
        {
            await validator.ValidateAndThrowAsync(request, ct);

            var category = new Category(currentUser.UserId, request.Name, request.Color);
            await EnsureNameIsUniqueAsync(category.Name, excludedId: null, ct);

            db.Categories.Add(category);
            await db.SaveChangesAsync(ct);

            return new CategoryResponse(category.Id, category.Name, category.Color);
        }

        public async Task<CategoryResponse?> UpdateAsync(Guid id, CategoryRequest request, CancellationToken ct)
        {
            await validator.ValidateAndThrowAsync(request, ct);

            var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == currentUser.UserId, ct);
            if (category is null) return null;

            category.Rename(request.Name);
            category.SetColor(request.Color);
            await EnsureNameIsUniqueAsync(category.Name, excludedId: id, ct);

            await db.SaveChangesAsync(ct);

            return new CategoryResponse(category.Id, category.Name, category.Color);

        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        {
            var exists = await db.Categories.AnyAsync(c => c.Id == id && c.UserId == currentUser.UserId , ct);

            if (!exists) return false;

            var isInUse = await db.Expenses.AnyAsync(e => e.CategoryId == id, ct) || await db.Budgets.AnyAsync(b => b.CategoryId == id, ct);

            if (isInUse)
            {
                throw new ConflictException("This category is used by expense or budgets and cannot be deleted.");
            }

            await db.Categories.Where(c => c.Id == id).ExecuteDeleteAsync(ct);

            return true;
        }

        private async Task EnsureNameIsUniqueAsync(string name, Guid? excludedId, CancellationToken ct)
        {
            var nameTaken = await db.Categories.AnyAsync(c => c.UserId == currentUser.UserId && c.Name == name && (excludedId == null || c.Id != excludedId), ct);

            if (nameTaken)
            {
                throw new ConflictException($"A category named '{name}' already exists.");
            }
        }
    }
}
