using Api.Core.Modules.SaleParticipants.Application.DTOs;
using Api.Core.Modules.SaleParticipants.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Core.Modules.SaleParticipants.Infrastructure.Persistence;

public sealed class SaleParticipantRepository : ISaleParticipantRepository
{
    private readonly AppDbContext _db;
    public SaleParticipantRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<SaleParticipantResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var participants = await _db.SaleParticipant
            .Include(p => p.Person)
            .ToListAsync(ct);

        return participants.Select(SaleParticipantResponse.FromEntity);
    }

    public async Task<SaleParticipantResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var participant = await _db.SaleParticipant
            .Include(p => p.Person)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        return participant is null ? null : SaleParticipantResponse.FromEntity(participant);
    }

    public Task<bool> SaleExistsAsync(int saleId, CancellationToken ct = default)
        => _db.Sale.AnyAsync(s => s.Id == saleId, ct);

    public Task<bool> PersonExistsAsync(int personId, CancellationToken ct = default)
        => _db.Person.AnyAsync(p => p.Id == personId, ct);

    public async Task<SaleParticipantResponse> CreateAsync(global::SaleParticipant participant, CancellationToken ct = default)
    {
        _db.SaleParticipant.Add(participant);
        await _db.SaveChangesAsync(ct);

        await _db.Entry(participant).Reference(p => p.Person).LoadAsync(ct);

        return SaleParticipantResponse.FromEntity(participant);
    }

    public async Task<bool> UpdateAsync(int id, string role, CancellationToken ct = default)
    {
        var participant = await _db.SaleParticipant.FindAsync([id], ct);
        if (participant is null) return false;

        participant.Role = role;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var participant = await _db.SaleParticipant.FindAsync([id], ct);
        if (participant is null) return false;

        _db.SaleParticipant.Remove(participant);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
