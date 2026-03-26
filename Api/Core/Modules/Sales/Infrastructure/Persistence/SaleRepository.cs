using Api.Core.Modules.Sales.Application.DTOs;
using Api.Core.Modules.Sales.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Core.Modules.Sales.Infrastructure.Persistence;

public sealed class SaleRepository : ISaleRepository
{
    private readonly AppDbContext _db;
    public SaleRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<SaleResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var sales = await _db.Sale
            .Include(s => s.Participants).ThenInclude(p => p.Person)
            .Include(s => s.Details).ThenInclude(d => d.Product)
            .ToListAsync(ct);

        return sales.Select(SaleResponse.FromEntity);
    }

    public async Task<SaleResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var sale = await _db.Sale
            .Include(s => s.Participants).ThenInclude(p => p.Person)
            .Include(s => s.Details).ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        return sale is null ? null : SaleResponse.FromEntity(sale);
    }

    public async Task<IReadOnlyList<int>> GetExistingPersonIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        return await _db.Person
            .Where(p => idList.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyDictionary<int, SaleProductInfo>> GetProductInfoAsync(IEnumerable<int> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        var products = await _db.Product
            .Where(p => idList.Contains(p.Id))
            .Select(p => new { p.Id, p.Stock, p.Price })
            .ToListAsync(ct);

        return products.ToDictionary(p => p.Id, p => new SaleProductInfo(p.Stock, p.Price));
    }

    public async Task<SaleResponse> CreateAsync(global::Sale sale, IReadOnlyDictionary<int, int> stockDeductions, CancellationToken ct = default)
    {
        // Apply stock deductions atomically with the sale
        if (stockDeductions.Count > 0)
        {
            var productIds = stockDeductions.Keys.ToList();
            var products = await _db.Product
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(ct);
            foreach (var product in products)
                if (stockDeductions.TryGetValue(product.Id, out var qty))
                    product.Stock -= qty;
        }

        _db.Sale.Add(sale);
        await _db.SaveChangesAsync(ct);

        // Reload nav props for the response
        foreach (var participant in sale.Participants)
            await _db.Entry(participant).Reference(p => p.Person).LoadAsync(ct);
        foreach (var detail in sale.Details)
            await _db.Entry(detail).Reference(d => d.Product).LoadAsync(ct);

        return SaleResponse.FromEntity(sale);
    }

    public async Task<bool> UpdateAsync(int id, string state, string? observations, CancellationToken ct = default)
    {
        var sale = await _db.Sale.FindAsync([id], ct);
        if (sale is null) return false;

        sale.State        = state;
        sale.Observations = observations;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var sale = await _db.Sale
            .Include(s => s.Details).ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        if (sale is null) return false;

        // Restore stock on delete
        foreach (var detail in sale.Details)
            if (detail.Product is not null)
                detail.Product.Stock += detail.Quantity;

        _db.Sale.Remove(sale);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
