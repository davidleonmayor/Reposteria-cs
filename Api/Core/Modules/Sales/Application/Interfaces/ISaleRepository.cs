using Api.Core.Modules.Sales.Application.DTOs;

namespace Api.Core.Modules.Sales.Application.Interfaces;

public sealed record SaleProductInfo(int Stock, decimal Price);

public interface ISaleRepository
{
    Task<IEnumerable<SaleResponse>> GetAllAsync(CancellationToken ct = default);
    Task<SaleResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<int>> GetExistingPersonIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);
    Task<IReadOnlyDictionary<int, SaleProductInfo>> GetProductInfoAsync(IEnumerable<int> ids, CancellationToken ct = default);
    Task<SaleResponse> CreateAsync(global::Sale sale, IReadOnlyDictionary<int, int> stockDeductions, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, string state, string? observations, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
