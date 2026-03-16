public interface ISaleDetailRepository
{
    Task<IEnumerable<SaleDetail>> GetAllAsync();
    Task<IEnumerable<SaleDetail>> GetBySaleIdAsync(int saleId);
    Task<SaleDetail?> GetByIdAsync(int id);
    Task<int> AddAsync(SaleDetail detail);
    Task<int> UpdateAsync(SaleDetail detail);
    Task<int> DeleteAsync(int id);
}
