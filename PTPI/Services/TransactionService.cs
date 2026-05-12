using PTPI.Models;
using PTPI.Repositories.Interfaces;
using PTPI.Services.Interfaces;

namespace PTPI.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountService _accountService;

        public TransactionService(ITransactionRepository transactionRepository, IAccountService accountService)
        {
            _transactionRepository = transactionRepository;
            _accountService = accountService;
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _transactionRepository.GetByIdAsync(id);
        }

        public async Task CreateTransactionAsync(Transaction transaction)
        {
            var account = await _accountService.GetAccountByIdAsync(transaction.AccountCode)
                ?? throw new InvalidOperationException("Account not found.");

            if (account.IsClosed)
                throw new InvalidOperationException("Cannot post transactions to a closed account.");

            if (transaction.Amount == 0)
                throw new InvalidOperationException("Transaction amount cannot be zero.");

            if (transaction.TransactionDate.Date > DateTime.Today)
                throw new InvalidOperationException("Transaction date cannot be in the future.");

            transaction.CaptureDate = DateTime.Now;
            await _transactionRepository.AddAsync(transaction);
            await _accountService.RecalculateBalanceAsync(transaction.AccountCode);
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            var existing = await _transactionRepository.FindAsync(transaction.Code)
                ?? throw new InvalidOperationException("Transaction not found.");

            var account = await _accountService.GetAccountByIdAsync(existing.AccountCode)
                ?? throw new InvalidOperationException("Account not found.");

            if (account.IsClosed)
                throw new InvalidOperationException("Cannot modify transactions on a closed account.");

            if (transaction.Amount == 0)
                throw new InvalidOperationException("Transaction amount cannot be zero.");

            if (transaction.TransactionDate.Date > DateTime.Today)
                throw new InvalidOperationException("Transaction date cannot be in the future.");

            existing.Description = transaction.Description;
            existing.Amount = transaction.Amount;
            existing.TransactionDate = transaction.TransactionDate;
            existing.CaptureDate = DateTime.Now;

            await _transactionRepository.SaveAsync();
            await _accountService.RecalculateBalanceAsync(existing.AccountCode);
        }
    }
}
