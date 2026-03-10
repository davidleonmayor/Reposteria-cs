using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class SaleController : ControllerBase
{
    private readonly AppDbContext _db;

    public SaleController(AppDbContext db) => _db = db;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var sales = await _db.Sale
            .Include(s => s.Participants)
                .ThenInclude(sp => sp.Person)
            .Include(s => s.Details)
                .ThenInclude(d => d.Product)
            .ToListAsync();

        return Ok(sales);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var sale = await _db.Sale
            .Include(s => s.Participants)
                .ThenInclude(sp => sp.Person)
            .Include(s => s.Details)
                .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

        return sale is null ? NotFound() : Ok(sale);
    }

    [HttpPost]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Create(SaleCreateDto dto)
    {
        var personIds = dto.Participants.Select(p => p.PersonId).Distinct().ToList();
        var productIds = dto.Details.Select(d => d.ProductId).Distinct().ToList();

        var existingPersonIds = await _db.Person
            .Where(p => personIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();

        var missingPersons = personIds.Except(existingPersonIds).ToList();
        if (missingPersons.Count > 0)
            return BadRequest(new { message = "Some persons do not exist.", missingPersons });

        var products = await _db.Product
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var missingProducts = productIds.Where(id => !products.ContainsKey(id)).ToList();
        if (missingProducts.Count > 0)
            return BadRequest(new { message = "Some products do not exist.", missingProducts });

        // Validate stock and build details
        var details = new List<SaleDetail>();
        foreach (var d in dto.Details)
        {
            var product = products[d.ProductId];
            var unitPrice = d.UnitPrice ?? product.Price;

            if (product.Stock < d.Quantity)
                return BadRequest(new { message = "Insufficient stock.", productId = product.Id, available = product.Stock, requested = d.Quantity });

            product.Stock -= d.Quantity;

            details.Add(new SaleDetail
            {
                ProductId = d.ProductId,
                Quantity = d.Quantity,
                UnitPrice = unitPrice
            });
        }

        var subtotal = details.Sum(x => x.UnitPrice * x.Quantity);

        var sale = new Sale
        {
            SaleDate = dto.SaleDate ?? DateTime.UtcNow,
            State = StringNormalization.Clean(dto.State),
            Observations = dto.Observations is null ? null : StringNormalization.Clean(dto.Observations),
            Subtotal = subtotal,
            Total = subtotal,
            Participants = dto.Participants
                .Select(p => new SaleParticipant { PersonId = p.PersonId, Role = StringNormalization.Clean(p.Role) })
                .ToList(),
            Details = details
        };

        _db.Sale.Add(sale);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Update(int id, SaleUpdateDto dto)
    {
        var sale = await _db.Sale.FindAsync(id);
        if (sale is null) return NotFound();

        sale.State = StringNormalization.Clean(dto.State);
        sale.Observations = dto.Observations is null ? null : StringNormalization.Clean(dto.Observations);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Delete(int id)
    {
        var sale = await _db.Sale
            .Include(s => s.Details)
            .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale is null) return NotFound();

        // Optional: restore stock on delete
        foreach (var d in sale.Details)
        {
            if (d.Product is not null)
                d.Product.Stock += d.Quantity;
        }

        _db.Sale.Remove(sale);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
