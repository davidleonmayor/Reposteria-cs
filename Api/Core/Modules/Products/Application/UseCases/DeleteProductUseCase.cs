using Api.Core.Modules.Products.Application.Interfaces;

namespace Api.Core.Modules.Products.Application.UseCases;

public sealed class DeleteProductUseCase
{
    private readonly IProductRepository _repository;

    public DeleteProductUseCase(IProductRepository repository) => _repository = repository;

    public Task<bool> ExecuteAsync(int id, CancellationToken cancellationToken = default)
        => _repository.DeleteAsync(id, cancellationToken);
}
