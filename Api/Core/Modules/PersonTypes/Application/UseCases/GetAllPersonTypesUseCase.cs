using Api.Core.Modules.PersonTypes.Application.DTOs;
using Api.Core.Modules.PersonTypes.Application.Interfaces;

namespace Api.Core.Modules.PersonTypes.Application.UseCases;

public sealed class GetAllPersonTypesUseCase
{
    private readonly IPersonTypeRepository _repository;

    public GetAllPersonTypesUseCase(IPersonTypeRepository repository) => _repository = repository;

    public Task<IEnumerable<PersonTypeResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);
}
