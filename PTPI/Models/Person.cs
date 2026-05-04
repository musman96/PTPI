using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTPI.Models
{
    [Table("Persons")]
    public class Person
    {
        [Key]
        [Column("code")]
        public int Code { get; set; }

        [Column("name", TypeName = "varchar(50)")]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Column("surname", TypeName = "varchar(50)")]
        [StringLength(50)]
        [Display(Name = "Surname")]
        public string? Surname { get; set; }

        [Column("id_number", TypeName = "varchar(13)")]
        [Required(ErrorMessage = "ID Number is required")]
        [StringLength(13)]
        [Display(Name = "ID Number")]
        public string IdNumber { get; set; } = string.Empty;

        public ICollection<Account> Accounts { get; set; } = new List<Account>();

        [NotMapped]
        public string FullName => $"{Name} {Surname}".Trim();
    }
}
