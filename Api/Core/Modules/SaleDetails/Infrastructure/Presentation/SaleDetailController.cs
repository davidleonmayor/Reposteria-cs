using Api.Core.Modules.SaleDetails.Application.DTOs;
using Api.Core.Modules.SaleDetails.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Core.Modules.SaleDetails.Infrastructure.Presentation;

[ApiController]
[Route("api/[controller]")]
public class SaleDetailController : ControllerBase
{
    private readonly GetAllSaleDetailsUseCase  _getAll;
    private readonly GetSaleDetailByIdUseCase  _getById;
    private readonly CreateSaleDetailUseCase   _create;
    private readonly UpdateSaleDetailUseCase   _update;
    private readonly DeleteSaleDetailUseCase   _delete;

    public SaleDetailController(
        GetAllSaleDetailsUseCase  getAll,
        GetSaleDetailByIdUseCase  getById,
        CreateSaleDetailUseCase   create,
        UpdateSaleDetailUseCase   update,
        DeleteSaleDetailUseCase   delete)
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
        var detail = await _getById.ExecuteAsync(id, cancellationToken);
        return detail is null ? NotFound() : Ok(detail);
    }

    [HttpPost]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Create(SaleDetailCreateRequest request, CancellationToken cancellationToken)
    {
        var detail = await _create.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = detail.Id }, detail);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Update(int id, SaleDetailUpdateRequest request, CancellationToken cancellationToken)
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
