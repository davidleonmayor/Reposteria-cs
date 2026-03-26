using Api.Core.Modules.SaleDetails.Application.DTOs;
using Api.Core.Modules.SaleDetails.Application.Interfaces;

namespace Api.Core.Modules.SaleDetails.Application.UseCases;

public sealed class UpdateSaleDetailUseCase
{
    private readonly ISaleDetailRepository _repository;
    public UpdateSaleDetailUseCase(ISaleDetailRepository repository) => _repository = repository;

    public Task<bool> ExecuteAsync(int id, SaleDetailUpdateRequest request, CancellationToken ct = default)
        => _repository.UpdateAsync(id, request.Quantity, request.UnitPrice, ct);
}
