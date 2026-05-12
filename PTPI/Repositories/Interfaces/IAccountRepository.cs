using PTPI.Models;

namespace PTPI.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int id);
        Task<Account?> FindAsync(int id);
        Task<bool> AccountNumberExistsAsync(string accountNumber, int? excludeCode = null);
        Task AddAsync(Account account);
        Task SaveAsync();
    }
}
