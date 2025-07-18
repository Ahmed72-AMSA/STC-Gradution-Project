using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BankSystem.Data.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;
        public string? Status { get; set; }


        [Required]
        [StringLength(6, MinimumLength = 6)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Account secret must be 6 digits")]
        public string? AccountSecret { get; set; }


        public decimal Balance { get; set; } = 0;
        public int UserID { get; set; }

        [JsonIgnore]
        public User User { get; set; } = null!;


        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
