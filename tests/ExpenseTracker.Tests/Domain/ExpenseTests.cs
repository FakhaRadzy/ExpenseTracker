using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Tests.Domain;

public class ExpenseTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid CategoryId = Guid.NewGuid();
    private static readonly DateOnly Today = new(2026, 9, 29);

    [Fact]
    public void Constructor_WithValidData_CreatesExpense()
    {
        var expense = new Expense(UserId, CategoryId, 25.50m, "  Lunch  ", Today, PaymentMethod.EWallet);

        Assert.Equal(25.50m, expense.Amount);
        Assert.Equal("Lunch", expense.Description); // trimmed
        Assert.NotEqual(Guid.Empty, expense.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WithNonPositiveAmount_Throws(int amount)
    {
        Assert.Throws<DomainException>(() =>
            new Expense(UserId, CategoryId, amount, "Lunch", Today, PaymentMethod.Cash));
    }

    [Fact]
    public void Constructor_WithMoreThanTwoDecimalPlaces_Throws()
    {
        Assert.Throws<DomainException>(() =>
            new Expense(UserId, CategoryId, 10.555m, "Lunch", Today, PaymentMethod.Cash));
    }

    [Fact]
    public void Constructor_WithEmptyDescription_Throws()
    {
        Assert.Throws<DomainException>(() =>
            new Expense(UserId, CategoryId, 10m, "   ", Today, PaymentMethod.Cash));
    }

    [Fact]
    public void Budget_WithInvalidMonth_Throws()
    {
        Assert.Throws<DomainException>(() => new Budget(UserId, CategoryId, 2026, 13, 500m));
    }

    [Fact]
    public void Budget_IsExceededBy_ReturnsTrueWhenOverLimit()
    {
        var budget = new Budget(UserId, CategoryId, 2026, 9, 500m);

        Assert.True(budget.IsExceededBy(500.01m));
        Assert.False(budget.IsExceededBy(500m));
    }
}