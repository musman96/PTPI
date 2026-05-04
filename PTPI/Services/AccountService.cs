using Microsoft.EntityFrameworkCore;
using PTPI.Data;
using PTPI.Models;
using PTPI.Services.Interfaces;

namespace PTPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetAccountByIdAsync(int id)
        {
            return await _context.Accounts
               // .Include(a => a.Person)
                .Include(a => a.Transactions.OrderByDescending(t => t.TransactionDate))
                .FirstOrDefaultAsync(a => a.Code == id);
        }

        public async Task CreateAccountAsync(Account account)
        {
            if (await IsAccountNumberTakenAsync(account.AccountNumber))
                throw new InvalidOperationException($"Account number '{account.AccountNumber}' already exists.");

            account.OutstandingBalance = 0;
            account.IsClosed = false;
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAccountAsync(Account account)
        {
            if (await IsAccountNumberTakenAsync(account.AccountNumber, account.Code))
                throw new InvalidOperationException($"Account number '{account.AccountNumber}' already exists.");

            var existing = await _context.Accounts.FindAsync(account.Code)
                ?? throw new InvalidOperationException("Account not found.");

            existing.AccountNumber = account.AccountNumber;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsAccountNumberTakenAsync(string accountNumber, int? excludeCode = null)
        {
            return await _context.Accounts.AnyAsync(a =>
                a.AccountNumber == accountNumber && (excludeCode == null || a.Code != excludeCode));
        }

        public async Task RecalculateBalanceAsync(int accountCode)
        {
            var account = await _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Code == accountCode)
                ?? throw new InvalidOperationException("Account not found.");

            account.OutstandingBalance = account.Transactions.Sum(t => t.Amount);
            await _context.SaveChangesAsync();
        }

        public async Task CloseAccountAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id)
                ?? throw new InvalidOperationException("Account not found.");

            if (account.OutstandingBalance != 0)
                throw new InvalidOperationException("Cannot close an account with a non-zero outstanding balance.");

            account.IsClosed = true;
            await _context.SaveChangesAsync();
        }

        public async Task ReopenAccountAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id)
                ?? throw new InvalidOperationException("Account not found.");

            account.IsClosed = false;
            await _context.SaveChangesAsync();
        }
    }
}
