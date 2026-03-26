using Api.Core.Modules.SaleParticipants.Application.DTOs;
using Api.Core.Modules.SaleParticipants.Application.Interfaces;

namespace Api.Core.Modules.SaleParticipants.Application.UseCases;

public sealed class UpdateSaleParticipantUseCase
{
    private readonly ISaleParticipantRepository _repository;
    public UpdateSaleParticipantUseCase(ISaleParticipantRepository repository) => _repository = repository;

    public Task<bool> ExecuteAsync(int id, SaleParticipantUpdateRequest request, CancellationToken ct = default)
    {
        var role = StringNormalization.Clean(request.Role);
        return _repository.UpdateAsync(id, role, ct);
    }
}
