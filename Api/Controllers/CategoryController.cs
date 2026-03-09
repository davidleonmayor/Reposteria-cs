using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoryController(AppDbContext db) => _db = db;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await _db.Category.ToListAsync());

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _db.Category.FindAsync(id);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Create(Category category)
    {
        _db.Category.Add(category);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Update(int id, Category updated)
    {
        var category = await _db.Category.FindAsync(id);
        if (category is null) return NotFound();
        category.Name = updated.Name;
        category.Description = updated.Description;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.Category.FindAsync(id);
        if (category is null) return NotFound();
        _db.Category.Remove(category);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

