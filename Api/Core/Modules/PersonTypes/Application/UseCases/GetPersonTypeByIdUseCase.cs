using Api.Core.Modules.PersonTypes.Application.DTOs;
using Api.Core.Modules.PersonTypes.Application.Interfaces;

namespace Api.Core.Modules.PersonTypes.Application.UseCases;

public sealed class GetPersonTypeByIdUseCase
{
    private readonly IPersonTypeRepository _repository;

    public GetPersonTypeByIdUseCase(IPersonTypeRepository repository) => _repository = repository;

    public Task<PersonTypeResponse?> ExecuteAsync(int id, CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(id, cancellationToken);
}
