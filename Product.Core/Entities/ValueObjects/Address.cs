using ProductApi.Core.Enums;
using ProductApi.Core.Shared;
using System.Text;

namespace ProductApi.Core.Entities.ValueObjects;

public sealed class Address : ValueObject
{
    public string Street1 { get; } = string.Empty;
    public string? Street2 { get; } = null;
    public string City { get; } = string.Empty;
    public string State { get; } = string.Empty;
    public string PostalCode { get; } = string.Empty;
    public string Country { get; } = string.Empty;
    public AddressType Type { get; }

    private Address() { }

    public Address(
        string street1,
        string city,
        string state,
        string postalCode,
        string country,
        AddressType type = AddressType.Shipping,
        string? street2 = null)
    {
        Street1 = ValidateString(street1, "Street address", 100);
        Street2 = ValidateOptionalString(street2, "Street address 2", 100);
        City = ValidateString(city, "City", 50);
        State = ValidateString(state, "State", 50);
        PostalCode = ValidateString(postalCode, "Postal code", 20);
        Country = ValidateString(country, "Country", 50);
        Type = type;
    }

    private static string ValidateString(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} is required");

        if (value.Length > maxLength)
            throw new ArgumentException($"{fieldName} cannot exceed {maxLength} characters");

        return value.Trim();
    }

    private static string? ValidateOptionalString(string? value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (value.Length > maxLength)
            throw new ArgumentException($"{fieldName} cannot exceed {maxLength} characters");

        return value.Trim();
    }

    public static Address CreateShipping(
        string street1, string city, string state,
        string postalCode, string country, string? street2 = null)
        => new(street1, city, state, postalCode, country, AddressType.Shipping, street2);

    public static Address CreateBilling(
        string street1, string city, string state,
        string postalCode, string country, string? street2 = null)
        => new(street1, city, state, postalCode, country, AddressType.Billing, street2);

    public string GetSingleLine()
    {
        var parts = new List<string> { Street1 };

        if (!string.IsNullOrEmpty(Street2))
            parts.Add(Street2);

        parts.AddRange([City, State, PostalCode, Country]);

        return string.Join(", ", parts.Where(p => !string.IsNullOrEmpty(p)));
    }

    public string GetMultiLine()
    {
        var builder = new StringBuilder();
        builder.AppendLine(Street1);

        if (!string.IsNullOrEmpty(Street2))
            builder.AppendLine(Street2);

        builder.AppendLine($"{City}, {State} {PostalCode}");
        builder.Append(Country);

        return builder.ToString();
    }

    public bool IsDomestic(string countryCode)
    {
        return string.Equals(Country, countryCode, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsValidForShipping()
    {
        return !string.IsNullOrEmpty(Street1) &&
               !string.IsNullOrEmpty(City) &&
               !string.IsNullOrEmpty(Country);
    }

    public bool Matches(Address other, bool strict = false)
    {
        if (strict)
        {
            return string.Equals(Street1, other.Street1, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Street2 ?? "", other.Street2 ?? "", StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(City, other.City, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(State, other.State, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(PostalCode, other.PostalCode, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Country, other.Country, StringComparison.OrdinalIgnoreCase);
        }

        return string.Equals(Street1, other.Street1, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(City, other.City, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(State, other.State, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(PostalCode, other.PostalCode, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Country, other.Country, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString() => GetSingleLine();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street1.ToUpperInvariant();
        yield return Street2?.ToUpperInvariant() ?? string.Empty;
        yield return City.ToUpperInvariant();
        yield return State.ToUpperInvariant();
        yield return PostalCode.ToUpperInvariant();
        yield return Country.ToUpperInvariant();
        yield return Type;
    }
}
