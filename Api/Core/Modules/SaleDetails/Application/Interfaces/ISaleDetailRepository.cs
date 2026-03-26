using Api.Core.Modules.SaleDetails.Application.DTOs;

namespace Api.Core.Modules.SaleDetails.Application.Interfaces;

public sealed record DetailProductInfo(int Stock, decimal Price);

public interface ISaleDetailRepository
{
    Task<IEnumerable<SaleDetailResponse>> GetAllAsync(CancellationToken ct = default);
    Task<SaleDetailResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> SaleExistsAsync(int saleId, CancellationToken ct = default);
    Task<DetailProductInfo?> GetProductInfoAsync(int productId, CancellationToken ct = default);
    Task<SaleDetailResponse> CreateAsync(global::SaleDetail detail, int stockDeduction, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, int newQuantity, decimal newUnitPrice, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
