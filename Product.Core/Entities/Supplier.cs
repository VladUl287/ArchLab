using ProductApi.Core.Entities.ValueObjects;
using ProductApi.Core.Shared;

namespace ProductApi.Core.Entities;

public class Supplier : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? ContactPerson { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public Address? Address { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Supplier(string name)
    {
        Name = name;
    }
}
