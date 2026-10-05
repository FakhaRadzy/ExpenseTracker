using ExpenseTracker.Application.Categories;
using ExpenseTracker.Application.Expenses;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IExpenseService, ExpenseService>();

            // Finds every AbstractValidator<T> in this project and registers it as IValidator<T>
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
