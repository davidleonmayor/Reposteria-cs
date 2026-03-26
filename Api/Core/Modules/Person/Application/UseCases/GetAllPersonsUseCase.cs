using Api.Core.Modules.Persons.Application.DTOs;
using Api.Core.Modules.Persons.Application.Interfaces;

namespace Api.Core.Modules.Persons.Application.UseCases;

public sealed class GetAllPersonsUseCase
{
    private readonly IPersonCrudRepository _repository;

    public GetAllPersonsUseCase(IPersonCrudRepository repository) => _repository = repository;

    public Task<IEnumerable<PersonResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);
}
