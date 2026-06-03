using Api.Core.Modules.Products.Application.Interfaces;

namespace Api.Core.Modules.Products.Application.UseCases;

public sealed class GetProductImageUseCase
{
    private readonly IProductRepository _repository;

    public GetProductImageUseCase(IProductRepository repository) => _repository = repository;

    public async Task<(byte[] Data, string ContentType)?> ExecuteAsync(int id, CancellationToken cancellationToken = default)
        => await _repository.GetImageAsync(id, cancellationToken);
}
