using PTPI.Models;

namespace PTPI.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Account?> GetAccountByIdAsync(int id);
        Task CreateAccountAsync(Account account);
        Task UpdateAccountAsync(Account account);
        Task<bool> IsAccountNumberTakenAsync(string accountNumber, int? excludeCode = null);
        Task RecalculateBalanceAsync(int accountCode);
        Task CloseAccountAsync(int id);
        Task ReopenAccountAsync(int id);
    }
}
