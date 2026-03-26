using Api.Core.Modules.Persons.Application.DTOs;
using Api.Core.Modules.Persons.Application.Interfaces;

namespace Api.Core.Modules.Persons.Application.UseCases;

public sealed class UpdatePersonUseCase
{
    private readonly IPersonCrudRepository _repository;

    public UpdatePersonUseCase(IPersonCrudRepository repository) => _repository = repository;

    public async Task<PersonResponse?> ExecuteAsync(int id, PersonUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var person = new global::Person
        {
            PersonTypeId = request.PersonTypeId,
            Name = StringNormalization.Clean(request.Name),
            LastName = StringNormalization.Clean(request.LastName),
            Phone = StringNormalization.Clean(request.Phone),
            Email = StringNormalization.Clean(request.Email),
            Address = StringNormalization.Clean(request.Address),
            RegisterDate = request.RegisterDate,
            Active = request.Active
        };

        return await _repository.UpdateAsync(id, person, cancellationToken);
    }
}
