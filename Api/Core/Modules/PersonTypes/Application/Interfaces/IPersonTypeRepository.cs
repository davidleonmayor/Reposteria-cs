using Api.Core.Modules.PersonTypes.Application.DTOs;

namespace Api.Core.Modules.PersonTypes.Application.Interfaces;

public interface IPersonTypeRepository
{
    Task<IEnumerable<PersonTypeResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PersonTypeResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PersonTypeResponse> AddAsync(global::PersonType personType, CancellationToken cancellationToken = default);
    Task<PersonTypeResponse?> UpdateAsync(int id, global::PersonType personType, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
