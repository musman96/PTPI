using Microsoft.EntityFrameworkCore;
using PTPI.Data;
using PTPI.Models;
using PTPI.Models.ViewModels;
using PTPI.Services.Interfaces;

namespace PTPI.Services
{
    public class PersonService : IPersonService
    {
        private readonly ApplicationDbContext _context;

        public PersonService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PersonListViewModel> GetPersonsAsync(string? searchTerm, int page)
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

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PersonListViewModel.PageSize);
            totalPages = Math.Max(1, totalPages);
            var currentPage = Math.Max(1, Math.Min(page, totalPages));

            var persons = await query
                .OrderBy(p => p.Surname)
                .ThenBy(p => p.Name)
                .Skip((currentPage - 1) * PersonListViewModel.PageSize)
                .Take(PersonListViewModel.PageSize)
                .ToListAsync();

            return new PersonListViewModel
            {
                Persons = persons,
                SearchTerm = searchTerm,
                CurrentPage = currentPage,
                TotalPages = totalPages,
                TotalCount = totalCount
            };
        }

        public async Task<Person?> GetPersonByIdAsync(int id)
        {
            return await _context.Persons
                .Include(p => p.Accounts)
                .FirstOrDefaultAsync(p => p.Code == id);
        }

        public async Task CreatePersonAsync(Person person)
        {
            if (await IsIdNumberTakenAsync(person.IdNumber))
                throw new InvalidOperationException($"A person with ID Number '{person.IdNumber}' already exists.");

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePersonAsync(Person person)
        {
            if (await IsIdNumberTakenAsync(person.IdNumber, person.Code))
                throw new InvalidOperationException($"A person with ID Number '{person.IdNumber}' already exists.");

            var existing = await _context.Persons.FindAsync(person.Code)
                ?? throw new InvalidOperationException("Person not found.");

            existing.Name = person.Name;
            existing.Surname = person.Surname;
            existing.IdNumber = person.IdNumber;

            await _context.SaveChangesAsync();
        }

        public async Task DeletePersonAsync(int id)
        {
            var person = await _context.Persons
                .Include(p => p.Accounts)
                .FirstOrDefaultAsync(p => p.Code == id)
                ?? throw new InvalidOperationException("Person not found.");

            if (person.Accounts.Any(a => !a.IsClosed))
                throw new InvalidOperationException("Cannot delete a person who has open accounts. Please close all accounts first.");

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsIdNumberTakenAsync(string idNumber, int? excludeCode = null)
        {
            return await _context.Persons.AnyAsync(p =>
                p.IdNumber == idNumber && (excludeCode == null || p.Code != excludeCode));
        }
    }
}
