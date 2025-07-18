namespace BankSystem.Service.Services
{
    public class TransactionReport
    {
        public string UserFullName { get; set; }
        public string AccountNumber { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
    }
}