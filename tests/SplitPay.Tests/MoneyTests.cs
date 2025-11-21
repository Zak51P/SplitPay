using SplitPay.Domain.ValueObjects;
using Xunit;

namespace SplitPay.Tests;

public class MoneyTests
{
    [Fact]
    public void NormalizesCurrencyAndRounds()
    {
        var money = new Money(10.129m, "usd");

        Assert.Equal(10.13m, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void Add_ThrowsOnCurrencyMismatch()
    {
        var a = new Money(5, "USD");
        var b = new Money(5, "EUR");

        Assert.Throws<InvalidOperationException>(() => a.Add(b));
    }
}
