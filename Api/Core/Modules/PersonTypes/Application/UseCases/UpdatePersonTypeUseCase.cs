using Api.Core.Modules.PersonTypes.Application.DTOs;
using Api.Core.Modules.PersonTypes.Application.Interfaces;

namespace Api.Core.Modules.PersonTypes.Application.UseCases;

public sealed class UpdatePersonTypeUseCase
{
    private readonly IPersonTypeRepository _repository;

    public UpdatePersonTypeUseCase(IPersonTypeRepository repository) => _repository = repository;

    public async Task<PersonTypeResponse?> ExecuteAsync(int id, PersonTypeUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var personType = new global::PersonType
        {
            Name        = StringNormalization.Clean(request.Name),
            Description = StringNormalization.Clean(request.Description)
        };

        return await _repository.UpdateAsync(id, personType, cancellationToken);
    }
}
