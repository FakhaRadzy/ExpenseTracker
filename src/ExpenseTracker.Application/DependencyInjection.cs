using ExpenseTracker.Application.Categories;
using ExpenseTracker.Application.Expenses;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IExpenseService, ExpenseService>();

            return services;
        }
    }
}
