using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductController(AppDbContext db) => _db = db;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await _db.Product.Include(p => p.Category).ToListAsync());

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _db.Product.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        var product = new Product
        {
            Name = StringNormalization.Clean(dto.Name),
            Description = StringNormalization.Clean(dto.Description),
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
            Active = dto.Active
        };

        if (product.CategoryId == 0)
        {
            var uncategorizedId = await _db.Category
                .Where(c => c.Name == "Uncategorized")
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (uncategorizedId == 0)
            {
                var category = new Category
                {
                    Name = "Uncategorized",
                    Description = "Auto-created"
                };

                _db.Category.Add(category);
                await _db.SaveChangesAsync();
                uncategorizedId = category.Id;
            }

            product.CategoryId = uncategorizedId;
        }

        _db.Product.Add(product);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
    {
        var product = await _db.Product.FindAsync(id);
        if (product is null) return NotFound();

        product.Name = StringNormalization.Clean(dto.Name);
        product.Description = StringNormalization.Clean(dto.Description);
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        if (dto.CategoryId != 0)
            product.CategoryId = dto.CategoryId;
        product.Active = dto.Active;

        await _db.SaveChangesAsync();
        return NoContent();

    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Product.FindAsync(id);
        if (product is null) return NotFound();
        _db.Product.Remove(product);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
