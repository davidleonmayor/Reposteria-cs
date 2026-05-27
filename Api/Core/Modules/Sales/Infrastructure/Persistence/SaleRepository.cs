using Api.Core.Modules.Sales.Application.DTOs;
using Api.Core.Modules.Sales.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Core.Modules.Sales.Infrastructure.Persistence;

public sealed class SaleRepository(AppDbContext db) : ISaleRepository
{
    public async Task<IEnumerable<SaleResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sales = await db.Sale
            .Include(s => s.Details).ThenInclude(d => d.Product)
            .ToListAsync(cancellationToken);

        return sales.Select(SaleResponse.FromEntity);
    }

    public async Task<SaleResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sale = await db.Sale
            .Include(s => s.Details).ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return sale is null ? null : SaleResponse.FromEntity(sale);
    }

    public async Task<SaleResponse> AddAsync(SaleCreateRequest request, CancellationToken cancellationToken = default)
    {
        var subtotal = request.Details.Sum(d => d.Quantity * d.UnitPrice);

        var sale = new Sale
        {
            SaleDate     = DateTime.UtcNow,
            State        = request.State,
            Observations = request.Observations,
            Subtotal     = subtotal,
            Total        = subtotal,
            Details      = request.Details.Select(d => new SaleDetail
            {
                ProductId = d.ProductId,
                Quantity  = d.Quantity,
                UnitPrice = d.UnitPrice
            }).ToList(),
            Participants = request.Participants.Select(p => new SaleParticipant
            {
                PersonId = p.PersonId,
                Role     = p.Role
            }).ToList()
        };

        db.Sale.Add(sale);
        await db.SaveChangesAsync(cancellationToken);

        await db.Entry(sale).Collection(s => s.Details)
            .Query().Include(d => d.Product)
            .LoadAsync(cancellationToken);

        return SaleResponse.FromEntity(sale);
    }

    public async Task<bool> UpdateAsync(int id, SaleUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var sale = await db.Sale.FindAsync([id], cancellationToken);
        if (sale is null) return false;

        sale.State        = request.State;
        sale.Observations = request.Observations;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var sale = await db.Sale.FindAsync([id], cancellationToken);
        if (sale is null) return false;

        db.Sale.Remove(sale);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
