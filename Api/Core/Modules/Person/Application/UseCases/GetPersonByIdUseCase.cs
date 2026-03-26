using Api.Core.Modules.Persons.Application.DTOs;
using Api.Core.Modules.Persons.Application.Interfaces;

namespace Api.Core.Modules.Persons.Application.UseCases;

public sealed class GetPersonByIdUseCase
{
    private readonly IPersonCrudRepository _repository;

    public GetPersonByIdUseCase(IPersonCrudRepository repository) => _repository = repository;

    public Task<PersonResponse?> ExecuteAsync(int id, CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(id, cancellationToken);
}
