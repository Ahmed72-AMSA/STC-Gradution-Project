using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankSystem.Data.Entities
{
    public class Cheque
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ChequeNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string SenderUserName { get; set; }

        [Required]
        [StringLength(100)]
        public string ReceiverName { get; set; }

        [StringLength(100)]
        public string ReceiverBankName { get; set; }

        [StringLength(50)]
        public string ReceiverAccountNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime IssueDate { get; set; } = DateTime.UtcNow;

        public DateTime? ClearanceDate { get; set; }

        public ChequeStatus Status { get; set; } = ChequeStatus.Pending;

        [ForeignKey("SenderUserName")]
        public virtual User Sender { get; set; }
    }

    public enum ChequeStatus
    {
        Pending,
        Cleared,
        Cancelled,
        Bounced
    }
}