using PTPI.Models;
using PTPI.Models.ViewModels;
using PTPI.Repositories.Interfaces;
using PTPI.Services.Interfaces;

namespace PTPI.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;

        public PersonService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<PersonListViewModel> GetPersonsAsync(string? searchTerm, int page)
        {
            var totalCount = await _personRepository.CountAsync(searchTerm);
            var totalPages = (int)Math.Ceiling(totalCount / (double)PersonListViewModel.PageSize);
            totalPages = Math.Max(1, totalPages);
            var currentPage = Math.Max(1, Math.Min(page, totalPages));

            var persons = await _personRepository.GetPagedAsync(searchTerm, currentPage, PersonListViewModel.PageSize);

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
            return await _personRepository.GetByIdAsync(id);
        }

        public async Task CreatePersonAsync(Person person)
        {
            if (await IsIdNumberTakenAsync(person.IdNumber))
                throw new InvalidOperationException($"A person with ID Number '{person.IdNumber}' already exists.");

            await _personRepository.AddAsync(person);
        }

        public async Task UpdatePersonAsync(Person person)
        {
            if (await IsIdNumberTakenAsync(person.IdNumber, person.Code))
                throw new InvalidOperationException($"A person with ID Number '{person.IdNumber}' already exists.");

            var existing = await _personRepository.GetByIdAsync(person.Code)
                ?? throw new InvalidOperationException("Person not found.");

            existing.Name = person.Name;
            existing.Surname = person.Surname;
            existing.IdNumber = person.IdNumber;

            await _personRepository.SaveAsync();
        }

        public async Task DeletePersonAsync(int id)
        {
            var person = await _personRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Person not found.");

            if (person.Accounts.Any(a => !a.IsClosed))
                throw new InvalidOperationException("Cannot delete a person who has open accounts. Please close all accounts first.");

            await _personRepository.DeleteAsync(person);
        }

        public async Task<bool> IsIdNumberTakenAsync(string idNumber, int? excludeCode = null)
        {
            return await _personRepository.IdNumberExistsAsync(idNumber, excludeCode);
        }
    }
}
