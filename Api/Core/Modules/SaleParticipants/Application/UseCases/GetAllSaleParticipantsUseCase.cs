using Api.Core.Modules.SaleParticipants.Application.DTOs;
using Api.Core.Modules.SaleParticipants.Application.Interfaces;

namespace Api.Core.Modules.SaleParticipants.Application.UseCases;

public sealed class GetAllSaleParticipantsUseCase
{
    private readonly ISaleParticipantRepository _repository;
    public GetAllSaleParticipantsUseCase(ISaleParticipantRepository repository) => _repository = repository;

    public Task<IEnumerable<SaleParticipantResponse>> ExecuteAsync(CancellationToken ct = default)
        => _repository.GetAllAsync(ct);
}
