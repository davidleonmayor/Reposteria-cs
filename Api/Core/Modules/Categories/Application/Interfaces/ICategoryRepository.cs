using Api.Core.Modules.Categories.Application.DTOs;

namespace Api.Core.Modules.Categories.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CategoryResponse> AddAsync(global::Category category, CancellationToken cancellationToken = default);
    Task<CategoryResponse?> UpdateAsync(int id, global::Category category, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
