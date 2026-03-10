public interface IPersonRepository
{
    Task<IEnumerable<Person>> GetAllAsync();
    Task<Person?> GetByIdAsync(int id);
    Task<int> AddAsync(Person person);
    Task<int> UpdateAsync(Person person);
    Task<int> DeleteAsync(int id);
}
