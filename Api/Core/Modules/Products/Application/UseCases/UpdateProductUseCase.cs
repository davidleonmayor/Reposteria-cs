using Api.Core.Modules.Products.Application.DTOs;
using Api.Core.Modules.Products.Application.Interfaces;

namespace Api.Core.Modules.Products.Application.UseCases;

public sealed class UpdateProductUseCase
{
    private readonly IProductRepository _repository;

    public UpdateProductUseCase(IProductRepository repository) => _repository = repository;

    public async Task<ProductResponse?> ExecuteAsync(int id, ProductUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var product = new global::Product
        {
            Name        = StringNormalization.Clean(request.Name),
            Description = StringNormalization.Clean(request.Description),
            Price       = request.Price,
            Stock       = request.Stock,
            // Regla de negocio: CategoryId == 0 → mantener el existente (repositorio aplica esta lógica)
            CategoryId  = request.CategoryId,
            Active      = request.Active
        };

        return await _repository.UpdateAsync(id, product, cancellationToken);
    }
}
