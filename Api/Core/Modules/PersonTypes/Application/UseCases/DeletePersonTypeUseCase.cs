using Api.Core.Modules.PersonTypes.Application.Interfaces;

namespace Api.Core.Modules.PersonTypes.Application.UseCases;

public sealed class DeletePersonTypeUseCase
{
    private readonly IPersonTypeRepository _repository;

    public DeletePersonTypeUseCase(IPersonTypeRepository repository) => _repository = repository;

    public Task<bool> ExecuteAsync(int id, CancellationToken cancellationToken = default)
        => _repository.DeleteAsync(id, cancellationToken);
}
