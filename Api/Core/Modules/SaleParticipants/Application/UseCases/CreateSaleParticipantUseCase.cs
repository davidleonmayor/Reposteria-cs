using Api.Core.Modules.SaleParticipants.Application.DTOs;
using Api.Core.Modules.SaleParticipants.Application.Interfaces;

namespace Api.Core.Modules.SaleParticipants.Application.UseCases;

public sealed class CreateSaleParticipantUseCase
{
    private readonly ISaleParticipantRepository _repository;
    public CreateSaleParticipantUseCase(ISaleParticipantRepository repository) => _repository = repository;

    public async Task<SaleParticipantResponse> ExecuteAsync(SaleParticipantCreateRequest request, CancellationToken ct = default)
    {
        if (!await _repository.SaleExistsAsync(request.SaleId, ct))
            throw new ArgumentException($"Sale {request.SaleId} not found.");

        if (!await _repository.PersonExistsAsync(request.PersonId, ct))
            throw new ArgumentException($"Person {request.PersonId} not found.");

        var participant = new global::SaleParticipant
        {
            SaleId   = request.SaleId,
            PersonId = request.PersonId,
            Role     = StringNormalization.Clean(request.Role)
        };

        return await _repository.CreateAsync(participant, ct);
    }
}
