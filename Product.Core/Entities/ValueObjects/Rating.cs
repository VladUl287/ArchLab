using ProductApi.Core.Shared;

namespace ProductApi.Core.Entities.ValueObjects;

public sealed class Rating : ValueObject
{
    public double Value { get; }
    public int StarCount => (int)Math.Round(Value * 2);

    private const double MinValue = 0.0;
    private const double MaxValue = 5.0;

    private Rating() { }

    public Rating(double value)
    {
        if (value < MinValue || value > MaxValue)
            throw new ArgumentException($"Rating must be between {MinValue} and {MaxValue}");

        Value = Math.Round(value, 1, MidpointRounding.AwayFromZero);
    }

    public static Rating FromStars(int stars)
    {
        if (stars < 0 || stars > 5)
            throw new ArgumentException("Stars must be between 0 and 5");

        return new Rating(stars);
    }

    public static Rating FromPercentage(double percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Percentage must be between 0 and 100");

        return new Rating((percentage / 100) * MaxValue);
    }
    
    public Rating Add(Rating other)
    {
        var newValue = (Value + other.Value) / 2;
        return new Rating(newValue);
    }

    public bool IsExcellent => Value >= 4.5;
    public bool IsGood => Value >= 4.0;
    public bool IsAverage => Value >= 3.0;
    public bool IsPoor => Value < 3.0;

    public override string ToString() => $"{Value:F1}/5.0";

    public static bool operator >(Rating left, Rating right) => left.Value > right.Value;
    public static bool operator <(Rating left, Rating right) => left.Value < right.Value;
    public static bool operator >=(Rating left, Rating right) => left.Value >= right.Value;
    public static bool operator <=(Rating left, Rating right) => left.Value <= right.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
