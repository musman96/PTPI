using PTPI.Models;

namespace PTPI.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(int id);
        Task<Transaction?> FindAsync(int id);
        Task AddAsync(Transaction transaction);
        Task SaveAsync();
    }
}
