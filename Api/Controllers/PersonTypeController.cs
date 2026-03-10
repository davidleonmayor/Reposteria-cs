using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class PersonTypeController : ControllerBase
{
    private readonly IPersonTypeRepository _repo;

    public PersonTypeController(IPersonTypeRepository repo) => _repo = repo;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpPost]
    [Authorize(Policy = "PeopleWrite")]
    public async Task<IActionResult> Create(PersonTypeCreateDto dto)
    {
        var personType = new PersonType
        {
            Name = StringNormalization.Clean(dto.Name),
            Description = StringNormalization.Clean(dto.Description)
        };

        var id = await _repo.AddAsync(personType);
        personType.Id = id;
        return CreatedAtAction(nameof(GetById), new { id }, personType);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var personType = await _repo.GetByIdAsync(id);
        return personType is null ? NotFound() : Ok(personType);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "PeopleWrite")]
    public async Task<IActionResult> Update(int id, PersonTypeUpdateDto dto)
    {
        var personType = await _repo.GetByIdAsync(id);
        if (personType is null) return NotFound();

        personType.Name = StringNormalization.Clean(dto.Name);
        personType.Description = StringNormalization.Clean(dto.Description);

        await _repo.UpdateAsync(personType);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "PeopleWrite")]
    public async Task<IActionResult> Delete(int id)
    {
        var affected = await _repo.DeleteAsync(id);
        return affected == 0 ? NotFound() : NoContent();
    }
}
