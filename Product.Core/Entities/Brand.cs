using ProductApi.Core.Entities.ValueObjects;
using ProductApi.Core.Shared;

namespace ProductApi.Core.Entities;

public sealed class Brand : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Slug { get; private set; } = string.Empty;
    public string? LogoUrl { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? ContactPhone { get; private set; }

    // Status
    public bool IsActive { get; private set; } = true;
    public bool IsVerified { get; private set; }
    public bool IsFeatured { get; private set; }

    // SEO
    public string? MetaTitle { get; private set; }
    public string? MetaDescription { get; private set; }
    public string? MetaKeywords { get; private set; }

    // Statistics
    public int ProductCount { get; private set; }
    public DateTime? LastProductAdded { get; private set; }
    public Rating AverageRating { get; private set; } = new(0);

    // Navigation
    private readonly List<Product> _products = [];
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Brand() : base() { }
}
