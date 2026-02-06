using ProductApi.Core.Enums;
using ProductApi.Core.Shared;

namespace ProductApi.Core.Entities;

public sealed class Product : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string ShortDescription { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;

    public Guid CategoryId { get; private set; }
    public Guid BrandId { get; private set; }

    public ProductStatus Status { get; private set; }
    public ConditionType Condition { get; private set; }
    public bool IsFeatured { get; private set; }
    public bool IsTaxable { get; private set; } = true;

    private readonly List<string> _imageUrls = [];
    public IReadOnlyCollection<string> ImageUrls => _imageUrls.AsReadOnly();

    private readonly List<string> _tags = [];
    public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();

    public string? MetaTitle { get; private set; }
    public string? MetaDescription { get; private set; }
    public string? MetaKeywords { get; private set; }

    public Category Category { get; private set; } = null!;
    public Brand Brand { get; private set; } = null!;
    private readonly List<ProductVariant> _variants = [];
    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();
    private readonly List<Review> _reviews = [];

    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();
}