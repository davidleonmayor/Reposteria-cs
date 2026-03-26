using Api.Core.Modules.Auth.Application.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Core.Modules.Auth.Application.Interfaces;

public interface ITokenGenerator
{
    Task<TokenResult> GenerateTokenAsync(Person person, CancellationToken cancellationToken = default);
}
