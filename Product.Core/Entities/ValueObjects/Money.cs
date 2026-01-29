using ProductApi.Core.Shared;

namespace ProductApi.Core.Entities.ValueObjects;

public sealed class Money() : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; } = string.Empty;
    public string Symbol => GetCurrencySymbol(Currency);

    public Money(decimal amount, string currency) : this()
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        return new Money(Amount * multiplier, Currency);
    }

    public override string ToString() => $"{Symbol}{Amount:0.00}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    private static string GetCurrencySymbol(string currency) =>
        currency switch
        {
            "USD" => "$",
            "EUR" => "€",
            "GBP" => "£",
            _ => currency
        };
}

