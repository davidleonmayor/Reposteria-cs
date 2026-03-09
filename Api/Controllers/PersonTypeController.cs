using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; 

[ApiController]
[Route("api/[controller]")]
public class PersonTypeController : ControllerBase {
  private readonly AppDbContext _db;

  public PersonTypeController(AppDbContext db) => _db = db;

  [HttpGet]
  [AllowAnonymous]
  public async Task<IActionResult> GetAll() => Ok(await _db.PersonType.ToListAsync());
  
  [HttpPost]
  [Authorize(Policy = "PeopleWrite")]
  public async Task<IActionResult> Create(PersonType personType) {
    _db.PersonType.Add(personType);
    await _db.SaveChangesAsync();
    return CreatedAtAction(nameof(GetById), new { id = personType.Id}, personType); 
  }

  [HttpGet("{id}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetById(int id) {
    var personType = await _db.PersonType.FindAsync(id);
    return personType is null ? NotFound() : Ok(personType); 
  }

  [HttpPut("{id}")]
  [Authorize(Policy = "PeopleWrite")]
  public async Task<IActionResult> Update(int id, PersonType updated) {
    var personType = await _db.PersonType.FindAsync(id);
    if (personType is null) return NotFound();
    personType.Name = updated.Name;
    personType.Description = updated.Description;
    await _db.SaveChangesAsync();
    return NoContent();
  }

  [HttpDelete("{id}")]
  [Authorize(Policy = "PeopleWrite")]
  public async Task<IActionResult> Delete(int id) {
    var personType = await _db.PersonType.FindAsync(id);
    if (personType is null) return NotFound();
    _db.PersonType.Remove(personType);
    await _db.SaveChangesAsync();
    return NoContent();
  }

}
