public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<int> AddAsync(Category category);
    Task<int> UpdateAsync(Category category);
    Task<int> DeleteAsync(int id);
}
