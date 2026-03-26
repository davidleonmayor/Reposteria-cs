using Api.Core.Modules.SaleDetails.Application.Interfaces;

namespace Api.Core.Modules.SaleDetails.Application.UseCases;

public sealed class DeleteSaleDetailUseCase
{
    private readonly ISaleDetailRepository _repository;
    public DeleteSaleDetailUseCase(ISaleDetailRepository repository) => _repository = repository;

    public Task<bool> ExecuteAsync(int id, CancellationToken ct = default)
        => _repository.DeleteAsync(id, ct);
}
