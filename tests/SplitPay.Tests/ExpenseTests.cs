using System.Linq;
using SplitPay.Domain.Entities;
using SplitPay.Domain.ValueObjects;
using Xunit;

namespace SplitPay.Tests;

public class ExpenseTests
{
    [Fact]
    public void EqualSplit_DistributesEvenly()
    {
        var expense = new Expense(Guid.NewGuid(), Guid.NewGuid(), new Money(100m, "USD"), "Dinner", SplitMethod.Equal);
        var members = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        var parts = members.Select(id => SplitPart.Exact(id, Money.Zero("USD"))).ToList();

        expense.SetParts(parts);

        Assert.All(expense.Parts, p => Assert.Equal(33.33m, p.Share.Amount));
    }

    [Fact]
    public void ExactSplit_WithWrongSum_Throws()
    {
        var expense = new Expense(Guid.NewGuid(), Guid.NewGuid(), new Money(100m, "USD"), "Test", SplitMethod.Exact);
        var parts = new[]
        {
            SplitPart.Exact(Guid.NewGuid(), new Money(40m, "USD")),
            SplitPart.Exact(Guid.NewGuid(), new Money(40m, "USD"))
        };

        Assert.Throws<InvalidOperationException>(() => expense.SetParts(parts));
    }

    [Fact]
    public void PercentSplit_WithWrongPercentSum_Throws()
    {
        var expense = new Expense(Guid.NewGuid(), Guid.NewGuid(), new Money(100m, "USD"), "Test", SplitMethod.Percent);
        var parts = new[]
        {
            SplitPart.PercentOf(Guid.NewGuid(), 60m, "USD"),
            SplitPart.PercentOf(Guid.NewGuid(), 50m, "USD")
        };

        Assert.Throws<InvalidOperationException>(() => expense.SetParts(parts));
    }

    [Fact]
    public void PercentSplit_ComputesShares()
    {
        var expense = new Expense(Guid.NewGuid(), Guid.NewGuid(), new Money(200m, "USD"), "Hotel", SplitMethod.Percent);
        var parts = new[]
        {
            SplitPart.PercentOf(Guid.NewGuid(), 25m, "USD"),
            SplitPart.PercentOf(Guid.NewGuid(), 75m, "USD")
        };

        expense.SetParts(parts);

        Assert.Equal(50m, expense.Parts.First().Share.Amount);
        Assert.Equal(150m, expense.Parts.Last().Share.Amount);
    }
}
