using Api.Core.Modules.Products.Application.DTOs;

namespace Api.Core.Modules.Products.Application.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<ProductResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductResponse> AddAsync(global::Product product, CancellationToken cancellationToken = default);
    Task<ProductResponse?> UpdateAsync(int id, global::Product product, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> UpdateImageAsync(int id, byte[] data, string contentType, CancellationToken cancellationToken = default);
    Task<(byte[] Data, string ContentType)?> GetImageAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca la categoría "Uncategorized" en la BD. Si no existe, la crea.
    /// Retorna su Id para asignarlo al producto cuando CategoryId == 0.
    /// </summary>
    Task<int> EnsureUncategorizedCategoryAsync(CancellationToken cancellationToken = default);
}
