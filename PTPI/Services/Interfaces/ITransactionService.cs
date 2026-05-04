using PTPI.Models;

namespace PTPI.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction?> GetTransactionByIdAsync(int id);
        Task CreateTransactionAsync(Transaction transaction);
        Task UpdateTransactionAsync(Transaction transaction);
    }
}
