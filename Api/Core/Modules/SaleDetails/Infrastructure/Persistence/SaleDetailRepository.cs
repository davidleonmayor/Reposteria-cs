using Api.Core.Modules.SaleDetails.Application.DTOs;
using Api.Core.Modules.SaleDetails.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Core.Modules.SaleDetails.Infrastructure.Persistence;

public sealed class SaleDetailRepository : ISaleDetailRepository
{
    private readonly AppDbContext _db;
    public SaleDetailRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<SaleDetailResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var details = await _db.SaleDetail
            .Include(d => d.Product)
            .ToListAsync(ct);

        return details.Select(SaleDetailResponse.FromEntity);
    }

    public async Task<SaleDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var detail = await _db.SaleDetail
            .Include(d => d.Product)
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        return detail is null ? null : SaleDetailResponse.FromEntity(detail);
    }

    public Task<bool> SaleExistsAsync(int saleId, CancellationToken ct = default)
        => _db.Sale.AnyAsync(s => s.Id == saleId, ct);

    public async Task<DetailProductInfo?> GetProductInfoAsync(int productId, CancellationToken ct = default)
    {
        var product = await _db.Product
            .Where(p => p.Id == productId)
            .Select(p => new { p.Stock, p.Price })
            .FirstOrDefaultAsync(ct);

        return product is null ? null : new DetailProductInfo(product.Stock, product.Price);
    }

    public async Task<SaleDetailResponse> CreateAsync(global::SaleDetail detail, int stockDeduction, CancellationToken ct = default)
    {
        // Deduct stock atomically with the insert
        var product = await _db.Product.FindAsync([detail.ProductId], ct);
        if (product is not null)
            product.Stock -= stockDeduction;

        _db.SaleDetail.Add(detail);
        await _db.SaveChangesAsync(ct);

        // Reload Product nav for the response
        await _db.Entry(detail).Reference(d => d.Product).LoadAsync(ct);

        return SaleDetailResponse.FromEntity(detail);
    }

    public async Task<bool> UpdateAsync(int id, int newQuantity, decimal newUnitPrice, CancellationToken ct = default)
    {
        var detail = await _db.SaleDetail
            .Include(d => d.Product)
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        if (detail is null) return false;

        // Adjust stock by the quantity delta
        var delta = newQuantity - detail.Quantity;
        if (delta != 0 && detail.Product is not null)
        {
            if (delta > 0 && detail.Product.Stock < delta)
                throw new ArgumentException(
                    $"Insufficient stock for product {detail.ProductId}. Available: {detail.Product.Stock}, Additional requested: {delta}");

            detail.Product.Stock -= delta;
        }

        detail.Quantity  = newQuantity;
        detail.UnitPrice = newUnitPrice;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var detail = await _db.SaleDetail
            .Include(d => d.Product)
            .FirstOrDefaultAsync(d => d.Id == id, ct);

        if (detail is null) return false;

        // Restore stock on delete
        if (detail.Product is not null)
            detail.Product.Stock += detail.Quantity;

        _db.SaleDetail.Remove(detail);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
