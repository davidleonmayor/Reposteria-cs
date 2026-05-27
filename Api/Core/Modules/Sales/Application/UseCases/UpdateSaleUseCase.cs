using Api.Core.Modules.Sales.Application.Interfaces;
using Api.Core.Modules.Sales.Application.DTOs;

namespace Api.Core.Modules.Sales.Application.UseCases;

public sealed class UpdateSaleUseCase(ISaleRepository repository)
{
    public Task<bool> ExecuteAsync(int id, SaleUpdateRequest request, CancellationToken cancellationToken = default)
        => repository.UpdateAsync(id, request, cancellationToken);
}
