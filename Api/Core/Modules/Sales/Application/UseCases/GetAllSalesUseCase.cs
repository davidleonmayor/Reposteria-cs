using Api.Core.Modules.Sales.Application.DTOs;
using Api.Core.Modules.Sales.Application.Interfaces;

namespace Api.Core.Modules.Sales.Application.UseCases;

public sealed class GetAllSalesUseCase(ISaleRepository repository)
{
    public Task<IEnumerable<SaleResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
        => repository.GetAllAsync(cancellationToken);
}
