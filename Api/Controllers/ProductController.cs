using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Product.Include(p => p.Categorys).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _db.Product.Include(p => p.Categorys).FirstOrDefaultAsync(p => p.Id == id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        _db.Product.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product updated)
    {
        var product = await _db.Product.FindAsync(id);
        product.Name = updated.Name;
        product.Description = updated.Description;
        product.Price = updated.Price;
        product.Stock = updated.Stock;
        product.Active = updated.Active;

        await _db.SaveChangesAsync();
        return NoContent();

    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Product.FindAsync(id);
        if (product is null) return NotFound();
        _db.Product.Remove(product);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}