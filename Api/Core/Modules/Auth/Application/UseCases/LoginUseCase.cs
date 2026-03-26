using Api.Core.Modules.Auth.Application.DTOs;
using Api.Core.Modules.Auth.Application.Interfaces;
using Api.Core.Modules.Auth.Application.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Core.Modules.Auth.Application.UseCases;

public sealed class LoginUseCase
{
    private readonly IPersonRepository _repository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUseCase(IPersonRepository repository, ITokenGenerator tokenGenerator, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _tokenGenerator = tokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponse?> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = StringNormalization.Clean(request.Email);
        var person = await _repository.GetByEmailAsync(email, cancellationToken);
        if (person is null)
            return null;
        if (!_passwordHasher.Verify(person.PasswordHash, request.Password))
            return null;

        var token = await _tokenGenerator.GenerateTokenAsync(person, cancellationToken);
        return new LoginResponse(token.Token, token.ExpiresAt);
    }
}
