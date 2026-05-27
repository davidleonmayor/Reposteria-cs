using Api.Core.Modules.Sales.Application.DTOs;

namespace Api.Core.Modules.Sales.Application.Interfaces;

public interface ISaleRepository
{
    Task<IEnumerable<SaleResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SaleResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SaleResponse> AddAsync(SaleCreateRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, SaleUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
