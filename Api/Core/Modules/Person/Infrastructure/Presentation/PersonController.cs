using Api.Core.Modules.Persons.Application.DTOs;
using Api.Core.Modules.Persons.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Core.Modules.Persons.Infrastructure.Presentation;

[ApiController]
[Route("api/[controller]")]
public class PersonController : ControllerBase
{
    private readonly GetAllPersonsUseCase _getAll;
    private readonly GetPersonByIdUseCase _getById;
    private readonly CreatePersonUseCase _create;
    private readonly UpdatePersonUseCase _update;
    private readonly DeletePersonUseCase _delete;

    public PersonController(
        GetAllPersonsUseCase getAll,
        GetPersonByIdUseCase getById,
        CreatePersonUseCase create,
        UpdatePersonUseCase update,
        DeletePersonUseCase delete)
    {
        _getAll = getAll;
        _getById = getById;
        _create = create;
        _update = update;
        _delete = delete;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _getAll.ExecuteAsync(cancellationToken));

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var person = await _getById.ExecuteAsync(id, cancellationToken);
        return person is null ? NotFound() : Ok(person);
    }

    [HttpPost]
    [Authorize(Policy = "PeopleWrite")]
    public async Task<IActionResult> Create(PersonCreateRequest request, CancellationToken cancellationToken)
    {
        var person = await _create.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "PeopleWrite")]
    public async Task<IActionResult> Update(int id, PersonUpdateRequest request, CancellationToken cancellationToken)
    {
        var person = await _update.ExecuteAsync(id, request, cancellationToken);
        return person is null ? NotFound() : NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "PeopleWrite")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _delete.ExecuteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
