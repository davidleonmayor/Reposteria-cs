using Api.Core.Modules.Sales.Application.DTOs;
using Api.Core.Modules.Sales.Application.Interfaces;

namespace Api.Core.Modules.Sales.Application.UseCases;

public sealed class GetAllSalesUseCase
{
    private readonly ISaleRepository _repository;
    public GetAllSalesUseCase(ISaleRepository repository) => _repository = repository;

    public Task<IEnumerable<SaleResponse>> ExecuteAsync(CancellationToken ct = default)
        => _repository.GetAllAsync(ct);
}
