using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _repo;

    public CategoryController(ICategoryRepository repo) => _repo = repo;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Create(CategoryCreateDto dto)
    {
        var category = new Category
        {
            Name = StringNormalization.Clean(dto.Name),
            Description = StringNormalization.Clean(dto.Description)
        };

        category.Id = await _repo.AddAsync(category);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Update(int id, CategoryUpdateDto dto)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category is null) return NotFound();
        category.Name = StringNormalization.Clean(dto.Name);
        category.Description = StringNormalization.Clean(dto.Description);

        await _repo.UpdateAsync(category);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Delete(int id)
    {
        var affected = await _repo.DeleteAsync(id);
        return affected == 0 ? NotFound() : NoContent();
    }
}

