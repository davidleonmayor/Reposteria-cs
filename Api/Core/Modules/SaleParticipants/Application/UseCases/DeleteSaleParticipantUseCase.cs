using Api.Core.Modules.SaleParticipants.Application.Interfaces;

namespace Api.Core.Modules.SaleParticipants.Application.UseCases;

public sealed class DeleteSaleParticipantUseCase
{
    private readonly ISaleParticipantRepository _repository;
    public DeleteSaleParticipantUseCase(ISaleParticipantRepository repository) => _repository = repository;

    public Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
        => _repository.DeleteAsync(id, ct);
}
