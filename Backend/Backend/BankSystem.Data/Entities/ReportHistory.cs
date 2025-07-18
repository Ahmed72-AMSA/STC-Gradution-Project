using System.ComponentModel.DataAnnotations;

namespace BankSystem.Data.Entities
{
    public class ReportHistory
    {
        [Key]
        public int Id { get; set; }

        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public string ReferenceNumber { get; set; }
        public string Status { get; set; }

        public byte[] PdfBytes { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}