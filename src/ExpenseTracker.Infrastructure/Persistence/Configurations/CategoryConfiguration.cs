using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ExpenseTracker.Infrastructure.Identity;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration: IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.HasOne<AppUser>().WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.Id).ValueGeneratedNever();

            builder.Property(c => c.Name).HasMaxLength(Category.NameMaxLength).IsRequired();

            builder.Property(c => c.Color).HasMaxLength(7); // "RRGGBB"

            // A user can't have two categories with the samne name
            builder.HasIndex(c => new { c.UserId, c.Name }).IsUnique();

            // Tell EF to fill the private_expenses list, since Expenses has no setter
            builder.Navigation(c => c.Expenses).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
