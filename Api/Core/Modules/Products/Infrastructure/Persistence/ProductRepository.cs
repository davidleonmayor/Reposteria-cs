using Api.Core.Modules.Products.Application.DTOs;
using Api.Core.Modules.Products.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Core.Modules.Products.Infrastructure.Persistence;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _db.Product
            .Include(p => p.Category)
            .ToListAsync(cancellationToken);

        return products.Select(ProductResponse.FromEntity);
    }

    public async Task<ProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _db.Product
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return product is null ? null : ProductResponse.FromEntity(product);
    }

    public async Task<ProductResponse> AddAsync(global::Product product, CancellationToken cancellationToken = default)
    {
        _db.Product.Add(product);
        await _db.SaveChangesAsync(cancellationToken);
        await _db.Entry(product).Reference(p => p.Category).LoadAsync(cancellationToken);
        return ProductResponse.FromEntity(product);
    }

    public async Task<ProductResponse?> UpdateAsync(int id, global::Product updated, CancellationToken cancellationToken = default)
    {
        var product = await _db.Product.FindAsync([id], cancellationToken);
        if (product is null) return null;

        product.Name        = updated.Name;
        product.Description = updated.Description;
        product.Price       = updated.Price;
        product.Stock       = updated.Stock;
        product.Active      = updated.Active;

        // Regla de negocio: CategoryId == 0 → conservar el existente
        if (updated.CategoryId != 0)
            product.CategoryId = updated.CategoryId;

        await _db.SaveChangesAsync(cancellationToken);
        await _db.Entry(product).Reference(p => p.Category).LoadAsync(cancellationToken);
        return ProductResponse.FromEntity(product);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _db.Product.FindAsync([id], cancellationToken);
        if (product is null) return false;

        _db.Product.Remove(product);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UpdateImageAsync(int id, byte[] data, string contentType, CancellationToken cancellationToken = default)
    {
        var product = await _db.Product.FindAsync([id], cancellationToken);
        if (product is null) return false;

        product.ImageData        = data;
        product.ImageContentType = contentType;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(byte[] Data, string ContentType)?> GetImageAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _db.Product
            .Where(p => p.Id == id)
            .Select(p => new { p.ImageData, p.ImageContentType })
            .FirstOrDefaultAsync(cancellationToken);

        if (product?.ImageData is null || product.ImageContentType is null)
            return null;

        return (product.ImageData, product.ImageContentType);
    }

    public async Task<int> EnsureUncategorizedCategoryAsync(CancellationToken cancellationToken = default)
    {
        var existingId = await _db.Category
            .Where(c => c.Name == "Uncategorized")
            .Select(c => c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingId != 0)
            return existingId;

        var category = new Category
        {
            Name        = "Uncategorized",
            Description = "Auto-created"
        };

        _db.Category.Add(category);
        await _db.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}
