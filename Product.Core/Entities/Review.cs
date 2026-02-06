using ProductApi.Core.Entities.ValueObjects;
using ProductApi.Core.Enums;
using ProductApi.Core.Shared;

namespace ProductApi.Core.Entities;

public sealed class Review : Entity
{
    public Rating Rating { get; private set; } = default!;
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;

    // Reviewer
    public Guid ReviewerId { get; private set; }
    public string ReviewerName { get; private set; } = string.Empty;
    public string? ReviewerEmail { get; private set; }

    // Product
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;

    // Status
    public ReviewStatus Status { get; private set; }
    public bool IsVerifiedPurchase { get; private set; }
    public bool IsHelpful { get; private set; }
    public int HelpfulCount { get; private set; }
    public int UnhelpfulCount { get; private set; }

    // Media
    private readonly List<string> _imageUrls = [];
    public IReadOnlyCollection<string> ImageUrls => _imageUrls.AsReadOnly();

    private readonly List<string> _videoUrls = [];
    public IReadOnlyCollection<string> VideoUrls => _videoUrls.AsReadOnly();

    private Review() : base() { }
}
