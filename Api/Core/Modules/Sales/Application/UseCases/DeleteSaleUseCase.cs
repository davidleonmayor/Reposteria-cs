using Api.Core.Modules.Sales.Application.Interfaces;

namespace Api.Core.Modules.Sales.Application.UseCases;

public sealed class DeleteSaleUseCase
{
    private readonly ISaleRepository _repository;
    public DeleteSaleUseCase(ISaleRepository repository) => _repository = repository;

    public Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
        => _repository.DeleteAsync(id, ct);
}
