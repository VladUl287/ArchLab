using ProductApi.Core.Entities;
using ProductApi.Core.Shared;

namespace ProductApi.Core.Aggregates;

public sealed class ProductAggregate : AggregateRoot
{
    private readonly List<ProductVariant> _variants = [];
    private readonly List<Review> _reviews = [];

    public IReadOnlyCollection<ProductVariant> Variants => _variants.AsReadOnly();
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    public void AddVariant(ProductVariant variant) { }
    public void UpdateStock(Guid variantId, int quantity) { }
    public void AddReview(Review review) { }
}
