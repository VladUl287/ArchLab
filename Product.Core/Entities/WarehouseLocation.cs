using ProductApi.Core.Entities.ValueObjects;
using ProductApi.Core.Shared;

namespace ProductApi.Core.Entities;

public class WarehouseLocation : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public Address Address { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    public string? ContactPerson { get; private set; }
    public string? ContactPhone { get; private set; }
    public string? ContactEmail { get; private set; }

    public WarehouseLocation(string name, string code, Address address)
    {
        Name = name;
        Code = code;
        Address = address;
    }
}
