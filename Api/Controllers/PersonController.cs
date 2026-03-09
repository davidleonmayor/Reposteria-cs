using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PersonController : ControllerBase
{
    private readonly AppDbContext _db;

    public PersonController(AppDbContext db) => _db = db;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await _db.Person.Include(p => p.PersonType).ToListAsync());

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var person = await _db.Person.Include(p => p.PersonType).FirstOrDefaultAsync(p => p.Id == id);
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

        _db.Person.Add(person);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "PeopleWrite")]
    public async Task<IActionResult> Update(int id, PersonUpdateDto dto)
    {
        var person = await _db.Person.FindAsync(id);
        if (person is null) return NotFound();
        person.PersonTypeId = dto.PersonTypeId;
        person.Name = StringNormalization.Clean(dto.Name);
        person.LastName = StringNormalization.Clean(dto.LastName);
        person.Phone = StringNormalization.Clean(dto.Phone);
        person.Email = StringNormalization.Clean(dto.Email);
        person.Address = StringNormalization.Clean(dto.Address);
        person.RegisterDate = dto.RegisterDate;
        person.Active = dto.Active;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "PeopleWrite")]
    public async Task<IActionResult> Delete(int id)
    {
        var person = await _db.Person.FindAsync(id);
        if (person is null) return NotFound();
        _db.Person.Remove(person);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
