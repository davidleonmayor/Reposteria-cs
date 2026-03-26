using Api.Core.Modules.Products.Application.DTOs;
using Api.Core.Modules.Products.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Core.Modules.Products.Infrastructure.Presentation;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly GetAllProductsUseCase  _getAll;
    private readonly GetProductByIdUseCase  _getById;
    private readonly CreateProductUseCase   _create;
    private readonly UpdateProductUseCase   _update;
    private readonly DeleteProductUseCase   _delete;

    public ProductController(
        GetAllProductsUseCase  getAll,
        GetProductByIdUseCase  getById,
        CreateProductUseCase   create,
        UpdateProductUseCase   update,
        DeleteProductUseCase   delete)
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
        var product = await _getById.ExecuteAsync(id, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Create(ProductCreateRequest request, CancellationToken cancellationToken)
    {
        var product = await _create.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Update(int id, ProductUpdateRequest request, CancellationToken cancellationToken)
    {
        var product = await _update.ExecuteAsync(id, request, cancellationToken);
        return product is null ? NotFound() : NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "CatalogWrite")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _delete.ExecuteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
