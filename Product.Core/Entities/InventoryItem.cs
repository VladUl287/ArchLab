using ProductApi.Core.Entities.ValueObjects;
using ProductApi.Core.Enums;
using ProductApi.Core.Shared;

namespace ProductApi.Core.Entities;

public class InventoryItem : Entity
{
    // Core
    public Guid ProductVariantId { get; private set; }
    public ProductVariant ProductVariant { get; private set; } = null!;

    // Identification
    public string? SerialNumber { get; private set; }
    public string? BatchNumber { get; private set; }
    public string? Barcode { get; private set; }
    public string? LotNumber { get; private set; }

    // Status
    public InventoryItemStatus Status { get; private set; }
    public InventoryItemCondition Condition { get; private set; }

    // Supplier & Cost
    public Guid? SupplierId { get; private set; }
    public Supplier? Supplier { get; private set; }
    public Money CostPrice { get; private set; } = default!;
    public string? PurchaseOrderNumber { get; private set; }
    public string? SupplierInvoiceNumber { get; private set; }

    // Dates
    public DateTime DateReceived { get; private set; }
    public DateTime? ManufactureDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DateTime? BestBeforeDate { get; private set; }
    public DateTime? DateSold { get; private set; }
    public DateTime? DateShipped { get; private set; }
    public DateTime? LastStockTakeDate { get; private set; }

    // Quality Control
    public bool IsQualityChecked { get; private set; }
    public string? QualityCheckedBy { get; private set; }
    public DateTime? QualityCheckDate { get; private set; }
    public string? QualityNotes { get; private set; }

    // Movement Tracking
    private readonly List<InventoryMovement> _movements = new();
    public IReadOnlyCollection<InventoryMovement> Movements => _movements.AsReadOnly();

    // Order Reference
    public Guid? OrderId { get; private set; }
    public Guid? OrderItemId { get; private set; }

    // Physical Properties
    public Dimensions? Dimensions { get; private set; }
    public decimal? Weight { get; private set; }
    public WeightUnit? WeightUnit { get; private set; }

    // Warranty
    public DateTime? WarrantyStartDate { get; private set; }
    public DateTime? WarrantyEndDate { get; private set; }
    public string? WarrantyTerms { get; private set; }

    private InventoryItem() : base() { }
}
