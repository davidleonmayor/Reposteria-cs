using Api.Core.Modules.SaleDetails.Application.DTOs;
using Api.Core.Modules.SaleDetails.Application.Interfaces;

namespace Api.Core.Modules.SaleDetails.Application.UseCases;

public sealed class GetSaleDetailByIdUseCase
{
    private readonly ISaleDetailRepository _repository;
    public GetSaleDetailByIdUseCase(ISaleDetailRepository repository) => _repository = repository;

    public Task<SaleDetailResponse?> ExecuteAsync(int id, CancellationToken ct = default)
        => _repository.GetByIdAsync(id, ct);
}
