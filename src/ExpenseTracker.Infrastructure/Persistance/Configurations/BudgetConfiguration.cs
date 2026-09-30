using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistance.Configurations
{
    public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builders)
        {
            builders.ToTable("Budgets", t => {
                t.HasCheckConstraint("CK_Budgets_Limit_Positive", "[Limit] > 0");
                t.HasCheckConstraint("CK_Budgets_Month_Range", "[Month] BETWEEN 1 AND 12");
                }
            );

            builders.HasKey(b => b.Id);
            builders.Property(b => b.Id).ValueGeneratedNever();

            builders.Property(b => b.Limit).HasPrecision(18, 2);

            builders.HasOne(b => b.Category).WithMany().HasForeignKey(b => b.CategoryId).OnDelete(DeleteBehavior.Restrict);

            // Only one budget per category per month for each user
            builders.HasIndex(b => new { b.UserId, b.CategoryId, b.Year, b.Month }).IsUnique();
          
            
        }
    }
}
