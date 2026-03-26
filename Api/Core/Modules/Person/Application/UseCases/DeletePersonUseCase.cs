using Api.Core.Modules.Persons.Application.Interfaces;

namespace Api.Core.Modules.Persons.Application.UseCases;

public sealed class DeletePersonUseCase
{
    private readonly IPersonCrudRepository _repository;

    public DeletePersonUseCase(IPersonCrudRepository repository) => _repository = repository;

    public Task<bool> ExecuteAsync(int id, CancellationToken cancellationToken = default)
        => _repository.DeleteAsync(id, cancellationToken);
}
