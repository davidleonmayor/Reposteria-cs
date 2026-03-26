using System.Threading;
using System.Threading.Tasks;

namespace Api.Core.Modules.Auth.Application.Interfaces;

public interface IPersonRepository
{
    Task<Person?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<Person> AddAsync(Person person, CancellationToken cancellationToken = default);
}
