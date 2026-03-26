using Api.Core.Modules.SaleParticipants.Application.DTOs;
using Api.Core.Modules.SaleParticipants.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Core.Modules.SaleParticipants.Infrastructure.Presentation;

[ApiController]
[Route("api/[controller]")]
public class SaleParticipantController : ControllerBase
{
    private readonly GetAllSaleParticipantsUseCase   _getAll;
    private readonly GetSaleParticipantByIdUseCase   _getById;
    private readonly CreateSaleParticipantUseCase    _create;
    private readonly UpdateSaleParticipantUseCase    _update;
    private readonly DeleteSaleParticipantUseCase    _delete;

    public SaleParticipantController(
        GetAllSaleParticipantsUseCase   getAll,
        GetSaleParticipantByIdUseCase   getById,
        CreateSaleParticipantUseCase    create,
        UpdateSaleParticipantUseCase    update,
        DeleteSaleParticipantUseCase    delete)
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
        var participant = await _getById.ExecuteAsync(id, cancellationToken);
        return participant is null ? NotFound() : Ok(participant);
    }

    [HttpPost]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Create(SaleParticipantCreateRequest request, CancellationToken cancellationToken)
    {
        var participant = await _create.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = participant.Id }, participant);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "SalesWrite")]
    public async Task<IActionResult> Update(int id, SaleParticipantUpdateRequest request, CancellationToken cancellationToken)
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
