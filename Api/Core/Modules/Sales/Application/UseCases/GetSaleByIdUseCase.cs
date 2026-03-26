using Api.Core.Modules.Sales.Application.DTOs;
using Api.Core.Modules.Sales.Application.Interfaces;

namespace Api.Core.Modules.Sales.Application.UseCases;

public sealed class GetSaleByIdUseCase
{
    private readonly ISaleRepository _repository;
    public GetSaleByIdUseCase(ISaleRepository repository) => _repository = repository;

    public Task<SaleResponse?> ExecuteAsync(int id, CancellationToken ct = default)
        => _repository.GetByIdAsync(id, ct);
}
