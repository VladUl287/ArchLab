using ProductApi.Core.Shared;

namespace ProductApi.Core.Entities;

public sealed class Product : Entity
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}