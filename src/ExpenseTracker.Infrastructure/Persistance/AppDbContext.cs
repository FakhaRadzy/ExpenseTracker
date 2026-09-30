using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace ExpenseTracker.Infrastructure.Persistance
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<Budget> Budgets => Set<Budget>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Finds every IEntityTypeConfiguration<T> class in this project and applies it
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
