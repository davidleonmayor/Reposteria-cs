using Api.Core.Modules.Products.Application.Interfaces;

namespace Api.Core.Modules.Products.Application.UseCases;

public sealed class UploadProductImageUseCase
{
    private readonly IProductRepository _repository;

    public UploadProductImageUseCase(IProductRepository repository) => _repository = repository;

    public async Task<bool> ExecuteAsync(int id, byte[] data, string contentType, CancellationToken cancellationToken = default)
        => await _repository.UpdateImageAsync(id, data, contentType, cancellationToken);
}
