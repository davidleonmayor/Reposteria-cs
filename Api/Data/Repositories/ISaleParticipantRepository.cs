public interface ISaleParticipantRepository
{
    Task<IEnumerable<SaleParticipant>> GetAllAsync();
    Task<IEnumerable<SaleParticipant>> GetBySaleIdAsync(int saleId);
    Task<SaleParticipant?> GetByIdAsync(int id);
    Task<int> AddAsync(SaleParticipant participant);
    Task<int> UpdateAsync(SaleParticipant participant);
    Task<int> DeleteAsync(int id);
}
