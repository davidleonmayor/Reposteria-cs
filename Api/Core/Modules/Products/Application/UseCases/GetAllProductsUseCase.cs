using Api.Core.Modules.Products.Application.DTOs;
using Api.Core.Modules.Products.Application.Interfaces;

namespace Api.Core.Modules.Products.Application.UseCases;

public sealed class GetAllProductsUseCase
{
    private readonly IProductRepository _repository;

    public GetAllProductsUseCase(IProductRepository repository) => _repository = repository;

    public Task<IEnumerable<ProductResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);
}
