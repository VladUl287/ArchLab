using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProductApi.Core.Entities;
using ProductApi.Core.Entities.ValueObjects;
using ProductApi.Core.Enums;
using ProductApi.Core.Shared;
using System.Text.Json;

namespace ProductApi.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<WarehouseLocation> WarehouseLocations => Set<WarehouseLocation>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        ConfigureValueObjects(modelBuilder);
        ConfigureRelationships(modelBuilder);
        ConfigureIndexes(modelBuilder);
        ConfigurePostgreSqlSettings(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        DispatchDomainEvents();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        DispatchDomainEvents();
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is Entity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            //if (entry.State == EntityState.Added)
            //{
            //    ((Entity)entry.Entity).CreatedAt = DateTime.UtcNow;
            //}
            //else if (entry.State == EntityState.Modified)
            //{
            //    ((Entity)entry.Entity).UpdatedAt = DateTime.UtcNow;
            //}
        }
    }

    private void DispatchDomainEvents()
    {
        var domainEntities = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents?.Any() == true)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            //publish
        }
    }

    private void ConfigureValueObjects(ModelBuilder modelBuilder)
    {
        ConfigureMoneyForEntity<ProductVariant>(modelBuilder);
        ConfigureMoneyForEntity<InventoryItem>(modelBuilder);

        modelBuilder.Entity<Review>()
            .OwnsOne(r => r.Rating, rating =>
            {
                rating.Property(r => r.Value)
                    .HasColumnName("RatingValue")
                    .HasColumnType("decimal(3,1)")
                    .IsRequired();
            });

        modelBuilder.Entity<WarehouseLocation>()
            .OwnsOne(w => w.Address, address =>
            {
                address.Property(a => a.Street1)
                    .HasColumnName("Street1")
                    .HasMaxLength(200)
                    .IsRequired();

                address.Property(a => a.Street2)
                    .HasColumnName("Street2")
                    .HasMaxLength(200);

                address.Property(a => a.City)
                    .HasColumnName("City")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.State)
                    .HasColumnName("State")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.PostalCode)
                    .HasColumnName("PostalCode")
                    .HasMaxLength(20)
                    .IsRequired();

                address.Property(a => a.Country)
                    .HasColumnName("Country")
                    .HasMaxLength(100)
                    .IsRequired();

                address.Property(a => a.Type)
                    .HasColumnName("AddressType")
                    .HasConversion<string>()
                    .HasMaxLength(20);
            });
    }

    private void ConfigureMoneyForEntity<T>(ModelBuilder modelBuilder) where T : class
    {
        modelBuilder.Entity<T>()
            .OwnsOne(typeof(T).Name + "Price", nameof(Money), money =>
            {

            });
    }

    private void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        // Product - ProductVariant (One-to-Many)
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Variants)
            .WithOne(pv => pv.Product)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Product - Review (One-to-Many)
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProductVariant - InventoryItem (One-to-Many)
        modelBuilder.Entity<ProductVariant>()
            .HasMany(pv => pv.InventoryItems)
            .WithOne(ii => ii.ProductVariant)
            .HasForeignKey(ii => ii.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        // InventoryItem - InventoryMovement (One-to-Many)
        modelBuilder.Entity<InventoryItem>()
            .HasMany(ii => ii.Movements)
            .WithOne(im => im.InventoryItem)
            .HasForeignKey(im => im.InventoryItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Category Hierarchy (Self-referencing)
        modelBuilder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Category - Product (One-to-Many)
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Brand - Product (One-to-Many)
        modelBuilder.Entity<Brand>()
            .HasMany(b => b.Products)
            .WithOne(p => p.Brand)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        // InventoryItem - WarehouseLocation (Many-to-One)
        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Location)
            .WithMany()
            .HasForeignKey(ii => ii.WarehouseLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // InventoryItem - Supplier (Many-to-One)
        modelBuilder.Entity<InventoryItem>()
            .HasOne(ii => ii.Supplier)
            .WithMany()
            .HasForeignKey(ii => ii.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Products_Slug");

        modelBuilder.Entity<Product>()
            .HasIndex(p => new { p.Status, p.IsFeatured })
            .HasDatabaseName("IX_Products_Status_Featured");

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.CreatedAt)
            .HasDatabaseName("IX_Products_CreatedAt");

        modelBuilder.Entity<ProductVariant>()
            .HasIndex(pv => pv.Sku)
            .IsUnique()
            .HasDatabaseName("IX_ProductVariants_SKU");

        modelBuilder.Entity<ProductVariant>()
            .HasIndex(pv => new { pv.ProductId, pv.IsActive })
            .HasDatabaseName("IX_ProductVariants_ProductId_Active");

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Categories_Slug");

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.ParentCategoryId)
            .HasDatabaseName("IX_Categories_ParentId");

        modelBuilder.Entity<Category>()
            .HasIndex(c => new { c.IsActive, c.DisplayOrder })
            .HasDatabaseName("IX_Categories_Active_DisplayOrder");

        modelBuilder.Entity<Brand>()
            .HasIndex(b => b.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Brands_Slug");

        modelBuilder.Entity<Brand>()
            .HasIndex(b => b.IsActive)
            .HasDatabaseName("IX_Brands_Active");

        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.ProductId, r.Status })
            .HasDatabaseName("IX_Reviews_ProductId_Status");

        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.ReviewerId, r.CreatedAt })
            .HasDatabaseName("IX_Reviews_ReviewerId_CreatedAt");

        modelBuilder.Entity<Review>()
            .HasIndex(r => r.Rating)
            .HasDatabaseName("IX_Reviews_Rating");

        modelBuilder.Entity<InventoryItem>()
            .HasIndex(ii => new { ii.ProductVariantId, ii.Status })
            .HasDatabaseName("IX_InventoryItems_VariantId_Status");

        modelBuilder.Entity<InventoryItem>()
            .HasIndex(ii => ii.SerialNumber)
            .IsUnique()
            .HasFilter("[SerialNumber] IS NOT NULL")
            .HasDatabaseName("IX_InventoryItems_SerialNumber");

        modelBuilder.Entity<InventoryItem>()
            .HasIndex(ii => ii.BatchNumber)
            .HasDatabaseName("IX_InventoryItems_BatchNumber");

        modelBuilder.Entity<InventoryItem>()
            .HasIndex(ii => ii.ExpiryDate)
            .HasDatabaseName("IX_InventoryItems_ExpiryDate");

        modelBuilder.Entity<InventoryItem>()
            .HasIndex(ii => new { ii.WarehouseLocationId, ii.Status })
            .HasDatabaseName("IX_InventoryItems_LocationId_Status");

        modelBuilder.Entity<InventoryMovement>()
            .HasIndex(im => new { im.InventoryItemId, im.MovementDate })
            .HasDatabaseName("IX_InventoryMovements_ItemId_Date");

        modelBuilder.Entity<InventoryMovement>()
            .HasIndex(im => im.Type)
            .HasDatabaseName("IX_InventoryMovements_Type");

        modelBuilder.Entity<Product>()
            .HasIndex(p => new { p.Name, p.Description, p.ShortDescription })
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops")
            .HasDatabaseName("IX_Products_FullTextSearch");
    }

    private void ConfigurePostgreSqlSettings(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<InventoryItemStatus>();
        modelBuilder.HasPostgresEnum<InventoryItemCondition>();
        modelBuilder.HasPostgresEnum<InventoryMovementType>();
        modelBuilder.HasPostgresEnum<ProductStatus>();
        modelBuilder.HasPostgresEnum<ConditionType>();
        modelBuilder.HasPostgresEnum<WeightUnit>();
        modelBuilder.HasPostgresEnum<ReviewStatus>();
        modelBuilder.HasPostgresEnum<DimensionUnit>();
        modelBuilder.HasPostgresEnum<AddressType>();

        // Configure JSONB columns for collections
        modelBuilder.Entity<Product>()
            .Property(p => p.Tags)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>());

        modelBuilder.Entity<Product>()
            .Property(p => p.ImageUrls)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>());

        modelBuilder.Entity<ProductVariant>()
            .Property(pv => pv.ImageUrls)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>());

        modelBuilder.Entity<Review>()
            .Property(r => r.ImageUrls)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>());

        modelBuilder.Entity<Review>()
            .Property(r => r.VideoUrls)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>());
    }
}