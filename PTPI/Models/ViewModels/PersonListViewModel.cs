namespace PTPI.Models.ViewModels
{
    public class PersonListViewModel
    {
        public IEnumerable<Person> Persons { get; set; } = new List<Person>();
        public string? SearchTerm { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public const int PageSize = 10;
    }
}
