using Microsoft.EntityFrameworkCore;
using PTPI.Data;
using PTPI.Models;
using PTPI.Repositories.Interfaces;

namespace PTPI.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the account with its transactions ordered by date descending.
        /// Used for the Account Details display page.
        /// </summary>
        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _context.Accounts
                .Include(a => a.Transactions.OrderByDescending(t => t.TransactionDate))
                .FirstOrDefaultAsync(a => a.Code == id);
        }

        /// <summary>
        /// Returns a bare Account entity without navigation properties.
        /// Used for lightweight operations such as close, reopen, and balance recalculation.
        /// </summary>
        public async Task<Account?> FindAsync(int id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task<bool> AccountNumberExistsAsync(string accountNumber, int? excludeCode = null)
        {
            return await _context.Accounts.AnyAsync(a =>
                a.AccountNumber == accountNumber && (excludeCode == null || a.Code != excludeCode));
        }

        public async Task AddAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
