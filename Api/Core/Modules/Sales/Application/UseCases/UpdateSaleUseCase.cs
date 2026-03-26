using Api.Core.Modules.Sales.Application.DTOs;
using Api.Core.Modules.Sales.Application.Interfaces;

namespace Api.Core.Modules.Sales.Application.UseCases;

public sealed class UpdateSaleUseCase
{
    private readonly ISaleRepository _repository;
    public UpdateSaleUseCase(ISaleRepository repository) => _repository = repository;

    public Task<bool> ExecuteAsync(int id, SaleUpdateRequest request, CancellationToken ct = default)
    {
        var state        = StringNormalization.Clean(request.State);
        var observations = request.Observations is null ? null : StringNormalization.Clean(request.Observations);
        return _repository.UpdateAsync(id, state, observations, ct);
    }
}
