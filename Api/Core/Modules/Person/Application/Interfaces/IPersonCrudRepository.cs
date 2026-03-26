using Api.Core.Modules.Persons.Application.DTOs;

namespace Api.Core.Modules.Persons.Application.Interfaces;

public interface IPersonCrudRepository
{
    Task<IEnumerable<PersonResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PersonResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PersonResponse> AddAsync(global::Person person, CancellationToken cancellationToken = default);
    Task<PersonResponse?> UpdateAsync(int id, global::Person person, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
