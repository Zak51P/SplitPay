namespace SplitPay.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency required", nameof(currency));
        Amount = Math.Round(amount, 2);
        Currency = currency.Trim().ToUpperInvariant();
    }

    public static Money Zero(string currency) => new(0m, currency);
    public Money Add(Money other) => Same(other) ? new(Amount + other.Amount, Currency) : throw new InvalidOperationException("Currency mismatch");
    public Money Subtract(Money other) => Same(other) ? new(Amount - other.Amount, Currency) : throw new InvalidOperationException("Currency mismatch");
    private bool Same(Money other) => string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase);
}
