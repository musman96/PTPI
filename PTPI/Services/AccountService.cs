using PTPI.Models;
using PTPI.Repositories.Interfaces;
using PTPI.Services.Interfaces;

namespace PTPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Account?> GetAccountByIdAsync(int id)
        {
            return await _accountRepository.GetByIdAsync(id);
        }

        public async Task CreateAccountAsync(Account account)
        {
            if (await IsAccountNumberTakenAsync(account.AccountNumber))
                throw new InvalidOperationException($"Account number '{account.AccountNumber}' already exists.");

            account.OutstandingBalance = 0;
            account.IsClosed = false;
            await _accountRepository.AddAsync(account);
        }

        public async Task UpdateAccountAsync(Account account)
        {
            if (await IsAccountNumberTakenAsync(account.AccountNumber, account.Code))
                throw new InvalidOperationException($"Account number '{account.AccountNumber}' already exists.");

            var existing = await _accountRepository.FindAsync(account.Code)
                ?? throw new InvalidOperationException("Account not found.");

            existing.AccountNumber = account.AccountNumber;
            await _accountRepository.SaveAsync();
        }

        public async Task<bool> IsAccountNumberTakenAsync(string accountNumber, int? excludeCode = null)
        {
            return await _accountRepository.AccountNumberExistsAsync(accountNumber, excludeCode);
        }

        public async Task RecalculateBalanceAsync(int accountCode)
        {
            var account = await _accountRepository.GetByIdAsync(accountCode)
                ?? throw new InvalidOperationException("Account not found.");

            account.OutstandingBalance = account.Transactions.Sum(t => t.Amount);
            await _accountRepository.SaveAsync();
        }

        public async Task CloseAccountAsync(int id)
        {
            var account = await _accountRepository.FindAsync(id)
                ?? throw new InvalidOperationException("Account not found.");

            if (account.OutstandingBalance != 0)
                throw new InvalidOperationException("Cannot close an account with a non-zero outstanding balance.");

            account.IsClosed = true;
            await _accountRepository.SaveAsync();
        }

        public async Task ReopenAccountAsync(int id)
        {
            var account = await _accountRepository.FindAsync(id)
                ?? throw new InvalidOperationException("Account not found.");

            account.IsClosed = false;
            await _accountRepository.SaveAsync();
        }
    }
}
