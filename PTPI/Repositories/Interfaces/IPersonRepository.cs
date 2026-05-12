using PTPI.Models;

namespace PTPI.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        Task<IEnumerable<Person>> GetPagedAsync(string? searchTerm, int page, int pageSize);
        Task<int> CountAsync(string? searchTerm);
        Task<Person?> GetByIdAsync(int id);
        Task<bool> IdNumberExistsAsync(string idNumber, int? excludeCode = null);
        Task AddAsync(Person person);
        Task SaveAsync();
        Task DeleteAsync(Person person);
    }
}
