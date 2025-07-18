

namespace BankSystem.Data.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public string SubscriptionName { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public bool IsActive { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int PartnerId { get; set; }
        public Partner Partner { get; set; }
    }
}
