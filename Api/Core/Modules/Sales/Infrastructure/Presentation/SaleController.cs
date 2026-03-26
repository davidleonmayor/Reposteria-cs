using Api.Core.Modules.Sales.Application.DTOs;
using Api.Core.Modules.Sales.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Core.Modules.Sales.Infrastructure.Presentation;

[ApiController]
[Route("api/[controller]")]
public class SaleController : ControllerBase
{
    private readonly GetAllSalesUseCase   _getAll;
    private readonly GetSaleByIdUseCase   _getById;
    private readonly CreateSaleUseCase    _create;
    private readonly UpdateSaleUseCase    _update;
    private readonly DeleteSaleUseCase    _delete;

    public SaleController(
        GetAllSalesUseCase   getAll,
        GetSaleByIdUseCase   getById,
        CreateSaleUseCase    create,
        UpdateSaleUseCase    update,
        DeleteSaleUseCase    delete)
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
        var sale = await _getById.ExecuteAsync(id, cancellationToken);
        return sale is null ? NotFound() : Ok(sale);
    }

    [HttpPost]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Create(SaleCreateRequest request, CancellationToken cancellationToken)
    {
        var sale = await _create.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Update(int id, SaleUpdateRequest request, CancellationToken cancellationToken)
    {
        var updated = await _update.ExecuteAsync(id, request, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _delete.ExecuteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
