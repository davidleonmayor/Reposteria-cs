using Api.Core.Modules.PersonTypes.Application.DTOs;
using Api.Core.Modules.PersonTypes.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Core.Modules.PersonTypes.Infrastructure.Persistence;

public sealed class PersonTypeRepository : IPersonTypeRepository
{
    private readonly AppDbContext _db;

    public PersonTypeRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<PersonTypeResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var types = await _db.PersonType.ToListAsync(cancellationToken);
        return types.Select(PersonTypeResponse.FromEntity);
    }

    public async Task<PersonTypeResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var personType = await _db.PersonType.FindAsync([id], cancellationToken);
        return personType is null ? null : PersonTypeResponse.FromEntity(personType);
    }

    public async Task<PersonTypeResponse> AddAsync(global::PersonType personType, CancellationToken cancellationToken = default)
    {
        _db.PersonType.Add(personType);
        await _db.SaveChangesAsync(cancellationToken);
        return PersonTypeResponse.FromEntity(personType);
    }

    public async Task<PersonTypeResponse?> UpdateAsync(int id, global::PersonType updated, CancellationToken cancellationToken = default)
    {
        var personType = await _db.PersonType.FindAsync([id], cancellationToken);
        if (personType is null) return null;

        personType.Name        = updated.Name;
        personType.Description = updated.Description;

        await _db.SaveChangesAsync(cancellationToken);
        return PersonTypeResponse.FromEntity(personType);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var personType = await _db.PersonType.FindAsync([id], cancellationToken);
        if (personType is null) return false;

        _db.PersonType.Remove(personType);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
