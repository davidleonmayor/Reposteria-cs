using Api.Core.Modules.SaleDetails.Application.DTOs;
using Api.Core.Modules.SaleDetails.Application.Interfaces;

namespace Api.Core.Modules.SaleDetails.Application.UseCases;

public sealed class GetAllSaleDetailsUseCase
{
    private readonly ISaleDetailRepository _repository;
    public GetAllSaleDetailsUseCase(ISaleDetailRepository repository) => _repository = repository;

    public Task<IEnumerable<SaleDetailResponse>> ExecuteAsync(CancellationToken ct = default)
        => _repository.GetAllAsync(ct);
}
