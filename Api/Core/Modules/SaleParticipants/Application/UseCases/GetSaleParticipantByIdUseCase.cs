using Api.Core.Modules.SaleParticipants.Application.DTOs;
using Api.Core.Modules.SaleParticipants.Application.Interfaces;

namespace Api.Core.Modules.SaleParticipants.Application.UseCases;

public sealed class GetSaleParticipantByIdUseCase
{
    private readonly ISaleParticipantRepository _repository;
    public GetSaleParticipantByIdUseCase(ISaleParticipantRepository repository) => _repository = repository;

    public Task<SaleParticipantResponse?> ExecuteAsync(int id, CancellationToken ct = default)
        => _repository.GetByIdAsync(id, ct);
}
