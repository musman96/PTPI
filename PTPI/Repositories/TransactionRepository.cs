using Microsoft.EntityFrameworkCore;
using PTPI.Data;
using PTPI.Models;
using PTPI.Repositories.Interfaces;

namespace PTPI.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the transaction with its parent Account included.
        /// Used for the Transaction Details display page.
        /// </summary>
        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.Code == id);
        }

        /// <summary>
        /// Returns a bare Transaction entity without navigation properties.
        /// Used for update operations where only the transaction fields are needed.
        /// </summary>
        public async Task<Transaction?> FindAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task AddAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
