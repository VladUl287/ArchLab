using ProductApi.Core.Entities.ValueObjects;
using ProductApi.Core.Enums;
using ProductApi.Core.Shared;

namespace ProductApi.Core.Entities;

public class ProductVariant : Entity
{
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; private set; }

    private readonly List<string> _imageUrls = [];
    public IReadOnlyCollection<string> ImageUrls => _imageUrls.AsReadOnly();

    public Money Price { get; private set; } = default!;
    public Money? CompareAtPrice { get; private set; }
    public Money CostPrice { get; private set; } = default!;

    private readonly List<InventoryItem> _inventoryItems = [];
    public IReadOnlyCollection<InventoryItem> InventoryItems => _inventoryItems.AsReadOnly();

    public bool IsActive { get; private set; } = true;
    public DateTime? AvailableFrom { get; private set; }
    public DateTime? AvailableUntil { get; private set; }

    public Dimensions? Dimensions { get; private set; }
    public decimal? Weight { get; private set; }
    public WeightUnit? WeightUnit { get; private set; }

    public Guid ProductId { get; init; }
    public Product? Product { get; init; }
}