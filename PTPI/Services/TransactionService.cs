using Microsoft.EntityFrameworkCore;
using PTPI.Data;
using PTPI.Models;
using PTPI.Services.Interfaces;

namespace PTPI.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;

        public TransactionService(ApplicationDbContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.Code == id);
        }

        public async Task CreateTransactionAsync(Transaction transaction)
        {
            var account = await _context.Accounts.FindAsync(transaction.AccountCode)
                ?? throw new InvalidOperationException("Account not found.");

            if (account.IsClosed)
                throw new InvalidOperationException("Cannot post transactions to a closed account.");

            if (transaction.Amount == 0)
                throw new InvalidOperationException("Transaction amount cannot be zero.");

            if (transaction.TransactionDate.Date > DateTime.Today)
                throw new InvalidOperationException("Transaction date cannot be in the future.");

            transaction.CaptureDate = DateTime.Now;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            await _accountService.RecalculateBalanceAsync(transaction.AccountCode);
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            var existing = await _context.Transactions.FindAsync(transaction.Code)
                ?? throw new InvalidOperationException("Transaction not found.");

            var account = await _context.Accounts.FindAsync(existing.AccountCode)
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

            await _context.SaveChangesAsync();
            await _accountService.RecalculateBalanceAsync(existing.AccountCode);
        }
    }
}
