using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTPI.Models
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        [Column("code")]
        public int Code { get; set; }

        [Column("person_code")]
        public int PersonCode { get; set; }

        [Column("account_number", TypeName = "varchar(50)")]
        [Required(ErrorMessage = "Account Number is required")]
        [StringLength(50)]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; } = string.Empty;

        [Column("outstanding_balance", TypeName = "money")]
        [Display(Name = "Outstanding Balance")]
        public decimal OutstandingBalance { get; set; }

        [Column("is_closed")]
        [Display(Name = "Closed")]
        public bool IsClosed { get; set; }

        [ForeignKey("PersonCode")]
        public Person Person { get; set; } = null!;

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
