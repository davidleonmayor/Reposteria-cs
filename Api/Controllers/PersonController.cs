using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PersonController : ControllerBase {
  private readonly AppDbContext _db;

  public PersonController(AppDbContext db) => _db = db;

  [HttpGet]
  public async Task<IActionResult> GetAll() => Ok(await _db.Person.Include(p => p.PersonType).ToListAsync());

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(int id) {
      var person = await _db.Person.Include(p => p.PersonType).FirstOrDefaultAsync(p => p.Id == id);
      return person is null ? NotFound() : Ok(person);
  }

  [HttpPost]
  public async Task<IActionResult> Create(Person person) {
      _db.Person.Add(person);
      await _db.SaveChangesAsync();
      return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> Update(int id, Person updated) {
      var person = await _db.Person.FindAsync(id);
      if (person is null) return NotFound();
      person.PersonTypeId = updated.PersonTypeId;
      person.Name = updated.Name;
      person.LastName = updated.LastName;
      person.Phone = updated.Phone;
      person.Email = updated.Email;
      person.Address = updated.Address;
      person.RegisterDate = updated.RegisterDate;
      person.Active = updated.Active;
      await _db.SaveChangesAsync();
      return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(int id) {
      var person = await _db.Person.FindAsync(id);
      if (person is null) return NotFound();
      _db.Person.Remove(person);
      await _db.SaveChangesAsync();
      return NoContent();
  }
}
