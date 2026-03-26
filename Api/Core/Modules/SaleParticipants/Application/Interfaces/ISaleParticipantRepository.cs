using Api.Core.Modules.SaleParticipants.Application.DTOs;

namespace Api.Core.Modules.SaleParticipants.Application.Interfaces;

public interface ISaleParticipantRepository
{
    Task<IEnumerable<SaleParticipantResponse>> GetAllAsync(CancellationToken ct = default);
    Task<SaleParticipantResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> SaleExistsAsync(int saleId, CancellationToken ct = default);
    Task<bool> PersonExistsAsync(int personId, CancellationToken ct = default);
    Task<SaleParticipantResponse> CreateAsync(global::SaleParticipant participant, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, string role, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
