using PTPI.Models;
using PTPI.Models.ViewModels;

namespace PTPI.Services.Interfaces
{
    public interface IPersonService
    {
        Task<PersonListViewModel> GetPersonsAsync(string? searchTerm, int page);
        Task<Person?> GetPersonByIdAsync(int id);
        Task CreatePersonAsync(Person person);
        Task UpdatePersonAsync(Person person);
        Task DeletePersonAsync(int id);
        Task<bool> IsIdNumberTakenAsync(string idNumber, int? excludeCode = null);
    }
}
