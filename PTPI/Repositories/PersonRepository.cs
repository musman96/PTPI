using Microsoft.EntityFrameworkCore;
using PTPI.Data;
using PTPI.Models;
using PTPI.Repositories.Interfaces;

namespace PTPI.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Person>> GetPagedAsync(string? searchTerm, int page, int pageSize)
        {
            return await BuildSearchQuery(searchTerm)
                .OrderBy(p => p.Surname)
                .ThenBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(string? searchTerm)
        {
            return await BuildSearchQuery(searchTerm).CountAsync();
        }

        public async Task<Person?> GetByIdAsync(int id)
        {
            return await _context.Persons
                .Include(p => p.Accounts)
                .FirstOrDefaultAsync(p => p.Code == id);
        }

        public async Task<bool> IdNumberExistsAsync(string idNumber, int? excludeCode = null)
        {
            return await _context.Persons.AnyAsync(p =>
                p.IdNumber == idNumber && (excludeCode == null || p.Code != excludeCode));
        }

        public async Task AddAsync(Person person)
        {
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Person person)
        {
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
        }

        private IQueryable<Person> BuildSearchQuery(string? searchTerm)
        {
            var query = _context.Persons.Include(p => p.Accounts).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.IdNumber.ToLower().Contains(term) ||
                    (p.Surname != null && p.Surname.ToLower().Contains(term)) ||
                    p.Accounts.Any(a => a.AccountNumber.ToLower().Contains(term)));
            }

            return query;
        }
    }
}
