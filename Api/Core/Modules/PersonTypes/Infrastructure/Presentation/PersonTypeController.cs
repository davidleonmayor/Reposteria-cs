using Api.Core.Modules.PersonTypes.Application.DTOs;
using Api.Core.Modules.PersonTypes.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Core.Modules.PersonTypes.Infrastructure.Presentation;

[ApiController]
[Route("api/[controller]")]
public class PersonTypeController : ControllerBase
{
    private readonly GetAllPersonTypesUseCase  _getAll;
    private readonly GetPersonTypeByIdUseCase  _getById;
    private readonly CreatePersonTypeUseCase   _create;
    private readonly UpdatePersonTypeUseCase   _update;
    private readonly DeletePersonTypeUseCase   _delete;

    public PersonTypeController(
        GetAllPersonTypesUseCase  getAll,
        GetPersonTypeByIdUseCase  getById,
        CreatePersonTypeUseCase   create,
        UpdatePersonTypeUseCase   update,
        DeletePersonTypeUseCase   delete)
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
        var personType = await _getById.ExecuteAsync(id, cancellationToken);
        return personType is null ? NotFound() : Ok(personType);
    }

    [HttpPost]
    [Authorize(Policy = "PeopleWrite")]
    public async Task<IActionResult> Create(PersonTypeCreateRequest request, CancellationToken cancellationToken)
    {
        var personType = await _create.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = personType.Id }, personType);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "PeopleWrite")]
    public async Task<IActionResult> Update(int id, PersonTypeUpdateRequest request, CancellationToken cancellationToken)
    {
        var personType = await _update.ExecuteAsync(id, request, cancellationToken);
        return personType is null ? NotFound() : NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "PeopleWrite")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _delete.ExecuteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
