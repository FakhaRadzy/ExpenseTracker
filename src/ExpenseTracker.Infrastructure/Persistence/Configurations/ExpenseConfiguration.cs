using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations
{
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expenses", t => t.HasCheckConstraint("CK_Expenses_Amount_Positive", "[Amount] > 0"));

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.Amount).HasPrecision(18, 2);

            builder.Property(e => e.Description).HasMaxLength(Expense.DescriptionMaxLength).IsRequired();

            builder.Property(e => e.Notes).HasMaxLength(Expense.NotesMaxLength);

            builder.HasOne(e => e.Category).WithMany(c => c.Expenses).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);

            // Most queries will be "my expenses, newer first" — this index makes that fast
            builder.HasIndex(e => new { e.UserId, e.Date });




        }
    }
}
