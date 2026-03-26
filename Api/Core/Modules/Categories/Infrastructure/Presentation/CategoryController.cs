using Api.Core.Modules.Categories.Application.DTOs;
using Api.Core.Modules.Categories.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Core.Modules.Categories.Infrastructure.Presentation;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly GetAllCategoriesUseCase _getAll;
    private readonly GetCategoryByIdUseCase  _getById;
    private readonly CreateCategoryUseCase   _create;
    private readonly UpdateCategoryUseCase   _update;
    private readonly DeleteCategoryUseCase   _delete;

    public CategoryController(
        GetAllCategoriesUseCase getAll,
        GetCategoryByIdUseCase  getById,
        CreateCategoryUseCase   create,
        UpdateCategoryUseCase   update,
        DeleteCategoryUseCase   delete)
    {
        _getAll  = getAll;
        _getById = getById;
        _create  = create;
        _update  = update;
        _delete  = delete;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _getAll.ExecuteAsync(cancellationToken));

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var category = await _getById.ExecuteAsync(id, cancellationToken);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Create(CategoryCreateRequest request, CancellationToken cancellationToken)
    {
        var category = await _create.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Update(int id, CategoryUpdateRequest request, CancellationToken cancellationToken)
    {
        var category = await _update.ExecuteAsync(id, request, cancellationToken);
        return category is null ? NotFound() : NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _delete.ExecuteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
