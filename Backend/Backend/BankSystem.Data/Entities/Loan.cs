
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankSystem.Data.Entities.Loans
{



    public class Loan
    {
        public int Id { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Annual income must be positive")]
        public decimal IncomeAnnum { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Loan amount must be positive")]
        public decimal LoanAmount { get; set; }

        [Required]
        [Range(1, 30, ErrorMessage = "Loan term must be between 1 and 30 years")]
        public int LoanTerm { get; set; }

        [Required]
        public string? Education { get; set; }

        [Required]
        public bool SelfEmployed { get; set; }

        public bool IsPaid { get; set; } = false;
        public string Status { get; set; } = "Pending";
        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovalDate { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public string? NationalIdDocumentPath { get; set; }
        public string? NationalIdDocumentHash { get; set; }
        public bool IsNationalIdVerified { get; set; } = false;
        public string? RejectionReason { get; set; }
    }
}
