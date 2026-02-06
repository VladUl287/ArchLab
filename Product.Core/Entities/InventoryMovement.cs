using ProductApi.Core.Enums;
using ProductApi.Core.Shared;

namespace ProductApi.Core.Entities;

public class InventoryMovement : Entity
{
    public Guid InventoryItemId { get; private set; }
    public InventoryMovementType Type { get; private set; }
    public DateTime MovementDate { get; private set; }
    public string Reference { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public Guid? FromLocationId { get; private set; }
    public Guid? ToLocationId { get; private set; }

    public InventoryItem InventoryItem { get; private set; } = null!;

    public InventoryMovement(
        InventoryMovementType type,
        string reference,
        string notes)
    {
        Type = type;
        Reference = reference;
        Notes = notes;
        MovementDate = DateTime.UtcNow;
    }
}

