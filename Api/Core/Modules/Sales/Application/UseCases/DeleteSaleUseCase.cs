using Api.Core.Modules.Sales.Application.Interfaces;

namespace Api.Core.Modules.Sales.Application.UseCases;

public sealed class DeleteSaleUseCase(ISaleRepository repository)
{
    public Task<bool> ExecuteAsync(int id, CancellationToken cancellationToken = default)
        => repository.DeleteAsync(id, cancellationToken);
}
