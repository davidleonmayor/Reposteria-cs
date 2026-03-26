using Api.Core.Modules.Products.Application.DTOs;
using Api.Core.Modules.Products.Application.Interfaces;

namespace Api.Core.Modules.Products.Application.UseCases;

public sealed class CreateProductUseCase
{
    private readonly IProductRepository _repository;

    public CreateProductUseCase(IProductRepository repository) => _repository = repository;

    public async Task<ProductResponse> ExecuteAsync(ProductCreateRequest request, CancellationToken cancellationToken = default)
    {
        var product = new global::Product
        {
            Name        = StringNormalization.Clean(request.Name),
            Description = StringNormalization.Clean(request.Description),
            Price       = request.Price,
            Stock       = request.Stock,
            CategoryId  = request.CategoryId,
            Active      = request.Active
        };

        // Regla de negocio: CategoryId == 0 → asignar/crear categoría "Uncategorized"
        if (product.CategoryId == 0)
            product.CategoryId = await _repository.EnsureUncategorizedCategoryAsync(cancellationToken);

        return await _repository.AddAsync(product, cancellationToken);
    }
}
