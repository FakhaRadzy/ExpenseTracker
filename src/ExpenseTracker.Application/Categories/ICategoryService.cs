using ExpenseTracker.Application.Categories;

namespace ExpenseTracker.Application.Categories
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken ct);
        Task<CategoryResponse?> GetIdAsync(Guid id, CancellationToken ct);
        Task<CategoryResponse> CreateAsync(CategoryRequest request, CancellationToken ct);
        Task<CategoryResponse?> UpdateAsync(Guid id, CategoryRequest request, CancellationToken ct);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    }
}
