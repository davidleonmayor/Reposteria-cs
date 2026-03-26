using Api.Core.Modules.Persons.Application.DTOs;
using Api.Core.Modules.Persons.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Core.Modules.Persons.Infrastructure.Persistence;

public sealed class PersonCrudRepository : IPersonCrudRepository
{
    private readonly AppDbContext _db;

    public PersonCrudRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<PersonResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var persons = await _db.Person
            .Include(p => p.PersonType)
            .ToListAsync(cancellationToken);

        return persons.Select(PersonResponse.FromEntity);
    }

    public async Task<PersonResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var person = await _db.Person
            .Include(p => p.PersonType)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return person is null ? null : PersonResponse.FromEntity(person);
    }

    public async Task<PersonResponse> AddAsync(global::Person person, CancellationToken cancellationToken = default)
    {
        _db.Person.Add(person);
        await _db.SaveChangesAsync(cancellationToken);
        await _db.Entry(person).Reference(p => p.PersonType).LoadAsync(cancellationToken);
        return PersonResponse.FromEntity(person);
    }

    public async Task<PersonResponse?> UpdateAsync(int id, global::Person updated, CancellationToken cancellationToken = default)
    {
        var person = await _db.Person.FindAsync([id], cancellationToken);
        if (person is null) return null;

        person.PersonTypeId = updated.PersonTypeId;
        person.Name = updated.Name;
        person.LastName = updated.LastName;
        person.Phone = updated.Phone;
        person.Email = updated.Email;
        person.Address = updated.Address;
        person.RegisterDate = updated.RegisterDate;
        person.Active = updated.Active;

        await _db.SaveChangesAsync(cancellationToken);
        await _db.Entry(person).Reference(p => p.PersonType).LoadAsync(cancellationToken);
        return PersonResponse.FromEntity(person);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var person = await _db.Person.FindAsync([id], cancellationToken);
        if (person is null) return false;

        _db.Person.Remove(person);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
