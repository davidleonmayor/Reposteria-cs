using Api.Core.Modules.Products.Application.DTOs;
using Api.Core.Modules.Products.Application.Interfaces;

namespace Api.Core.Modules.Products.Application.UseCases;

public sealed class GetProductByIdUseCase
{
    private readonly IProductRepository _repository;

    public GetProductByIdUseCase(IProductRepository repository) => _repository = repository;

    public Task<ProductResponse?> ExecuteAsync(int id, CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(id, cancellationToken);
}
