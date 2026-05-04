using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTPI.Models
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        [Column("code")]
        public int Code { get; set; }

        [Column("account_code")]
        public int AccountCode { get; set; }

        [Column("transaction_date", TypeName = "datetime")]
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }

        [Column("capture_date", TypeName = "datetime")]
        [Display(Name = "Capture Date")]
        public DateTime CaptureDate { get; set; }

        [Column("amount", TypeName = "money")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Column("description", TypeName = "varchar(100)")]
        [Required(ErrorMessage = "Description is required")]
        [StringLength(100)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [ForeignKey("AccountCode")]
        public Account Account { get; set; } = null!;
    }
}
