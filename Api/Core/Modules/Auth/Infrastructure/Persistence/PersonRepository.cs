using Api.Core.Modules.Auth.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Core.Modules.Auth.Infrastructure.Persistence;

public sealed class PersonRepository : IPersonRepository
{
    private readonly AppDbContext _db;

    public PersonRepository(AppDbContext db) => _db = db;

    public async Task<Person?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _db.Person
            .Include(p => p.PersonType)
            .FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _db.Person.AnyAsync(p => p.Email == email, cancellationToken);
    }

    public async Task<Person> AddAsync(Person person, CancellationToken cancellationToken = default)
    {
        _db.Person.Add(person);
        await _db.SaveChangesAsync(cancellationToken);
        await _db.Entry(person).Reference(p => p.PersonType).LoadAsync(cancellationToken);
        return person;
    }
}
