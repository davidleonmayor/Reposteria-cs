using Api.Core.Modules.Sales.Application.DTOs;
using Api.Core.Modules.Sales.Application.Interfaces;

namespace Api.Core.Modules.Sales.Application.UseCases;

public sealed class CreateSaleUseCase(ISaleRepository repository)
{
    public Task<SaleResponse> ExecuteAsync(SaleCreateRequest request, CancellationToken cancellationToken = default)
        => repository.AddAsync(request, cancellationToken);
}
