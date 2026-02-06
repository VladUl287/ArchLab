using ProductApi.Core.Shared;

namespace ProductApi.Core.Entities;

public sealed class Category : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? ImageUrl { get; private set; }
    public string? IconClass { get; private set; }

    public Guid? ParentCategoryId { get; private set; }
    public Category? ParentCategory { get; private set; }

    private readonly List<Category> _subcategories = [];
    public IReadOnlyCollection<Category> Subcategories => _subcategories.AsReadOnly();

    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsFeatured { get; private set; }

    public string? MetaTitle { get; private set; }
    public string? MetaDescription { get; private set; }
    public string? MetaKeywords { get; private set; }

    private readonly List<Product> _products = [];
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();
}
