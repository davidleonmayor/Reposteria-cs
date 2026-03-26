using Api.Core.Modules.Auth.Application.DTOs;
using Api.Core.Modules.Auth.Application.Interfaces;
using Api.Core.Modules.Auth.Application.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Core.Modules.Auth.Application.UseCases;

public sealed class RegisterUseCase
{
    private readonly IPersonRepository _personRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUseCase(IPersonRepository personRepository, ITokenGenerator tokenGenerator, IPasswordHasher passwordHasher)
    {
        _personRepository = personRepository;
        _tokenGenerator = tokenGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponse?> ExecuteAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = StringNormalization.Clean(request.Email);
        if (await _personRepository.EmailExistsAsync(email, cancellationToken))
            return null;

        var person = new Person
        {
            Email = email,
            Name = StringNormalization.Clean(request.Name),
            LastName = StringNormalization.Clean(request.LastName),
            Phone = StringNormalization.Clean(request.Phone ?? string.Empty),
            Address = StringNormalization.Clean(request.Address ?? string.Empty),
            PersonTypeId = request.PersonTypeId,
            RegisterDate = DateTime.UtcNow,
            Active = true
        };

        person.PasswordHash = _passwordHasher.Hash(request.Password);

        var created = await _personRepository.AddAsync(person, cancellationToken);
        var token = await _tokenGenerator.GenerateTokenAsync(created, cancellationToken);
        return new LoginResponse(token.Token, token.ExpiresAt);
    }
}
