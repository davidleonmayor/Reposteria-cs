using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class PersonController : ControllerBase
{
    private readonly IPersonRepository _repo;

    public PersonController(IPersonRepository repo) => _repo = repo;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var person = await _repo.GetByIdAsync(id);
        return person is null ? NotFound() : Ok(person);
    }

    [HttpPost]
    [Authorize(Policy = "PeopleWrite")]
    public async Task<IActionResult> Create(PersonCreateDto dto)
    {
        var person = new Person
        {
            PersonTypeId = dto.PersonTypeId,
            Name = StringNormalization.Clean(dto.Name),
            LastName = StringNormalization.Clean(dto.LastName),
            Phone = StringNormalization.Clean(dto.Phone),
            Email = StringNormalization.Clean(dto.Email),
            Address = StringNormalization.Clean(dto.Address),
            RegisterDate = dto.RegisterDate ?? DateTime.UtcNow,
            Active = dto.Active
        };

        person.Id = await _repo.AddAsync(person);
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "PeopleWrite")]
    public async Task<IActionResult> Update(int id, PersonUpdateDto dto)
    {
        var person = await _repo.GetByIdAsync(id);
        if (person is null) return NotFound();

        person.PersonTypeId = dto.PersonTypeId;
        person.Name = StringNormalization.Clean(dto.Name);
        person.LastName = StringNormalization.Clean(dto.LastName);
        person.Phone = StringNormalization.Clean(dto.Phone);
        person.Email = StringNormalization.Clean(dto.Email);
        person.Address = StringNormalization.Clean(dto.Address);
        person.RegisterDate = dto.RegisterDate;
        person.Active = dto.Active;

        await _repo.UpdateAsync(person);
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
