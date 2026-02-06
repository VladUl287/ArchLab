using ProductApi.Core.Enums;
using ProductApi.Core.Shared;

namespace ProductApi.Core.Entities.ValueObjects;

public sealed class Dimensions : ValueObject
{
    public decimal Length { get; }
    public decimal Width { get; }
    public decimal Height { get; }
    public DimensionUnit Unit { get; }

    private Dimensions() { }

    public Dimensions(decimal length, decimal width, decimal height, DimensionUnit unit = DimensionUnit.Centimeters)
    {
        if (length <= 0 || width <= 0 || height <= 0)
            throw new ArgumentException("Dimensions must be positive values");

        Length = Math.Round(length, 2);
        Width = Math.Round(width, 2);
        Height = Math.Round(height, 2);
        Unit = unit;
    }

    public decimal Volume => Length * Width * Height;
    public decimal VolumeInLiters => Unit == DimensionUnit.Centimeters ? Volume / 1000 : Volume;

    public decimal Girth => (2 * Width) + (2 * Height);
    public decimal LengthPlusGirth => Length + Girth;

    public static Dimensions InCentimeters(decimal length, decimal width, decimal height)
        => new(length, width, height, DimensionUnit.Centimeters);

    public static Dimensions InInches(decimal length, decimal width, decimal height)
        => new(length, width, height, DimensionUnit.Inches);

    public static Dimensions InMeters(decimal length, decimal width, decimal height)
        => new(length, width, height, DimensionUnit.Meters);

    public Dimensions ConvertTo(DimensionUnit targetUnit)
    {
        if (Unit == targetUnit)
            return this;

        var conversionFactor = GetConversionFactor(Unit, targetUnit);
        return new Dimensions(
            Length * conversionFactor,
            Width * conversionFactor,
            Height * conversionFactor,
            targetUnit);
    }

    private static decimal GetConversionFactor(DimensionUnit from, DimensionUnit to)
    {
        var inCentimeters = from switch
        {
            DimensionUnit.Centimeters => 1m,
            DimensionUnit.Inches => 2.54m,
            DimensionUnit.Meters => 100m,
            DimensionUnit.Millimeters => 0.1m,
            _ => throw new ArgumentOutOfRangeException(nameof(from))
        };

        var target = to switch
        {
            DimensionUnit.Centimeters => 1m,
            DimensionUnit.Inches => 1m / 2.54m,
            DimensionUnit.Meters => 0.01m,
            DimensionUnit.Millimeters => 10m,
            _ => throw new ArgumentOutOfRangeException(nameof(to))
        };

        return inCentimeters * target;
    }

    public decimal GetVolumetricWeight(decimal factor = 5000)
    {
        return Volume / factor;
    }

    public bool IsOversized()
    {
        return Unit switch
        {
            DimensionUnit.Inches =>
                Length > 96 || Width > 96 || Height > 96 ||
                LengthPlusGirth > 165,
            DimensionUnit.Centimeters =>
                Length > 244 || Width > 244 || Height > 244 ||
                LengthPlusGirth > 330,
            _ => false
        };
    }

    public override string ToString()
    {
        var unitAbbr = Unit switch
        {
            DimensionUnit.Centimeters => "cm",
            DimensionUnit.Inches => "in",
            DimensionUnit.Meters => "m",
            DimensionUnit.Millimeters => "mm",
            _ => ""
        };

        return $"{Length:N1} × {Width:N1} × {Height:N1} {unitAbbr}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Length;
        yield return Width;
        yield return Height;
        yield return Unit;
    }
}
