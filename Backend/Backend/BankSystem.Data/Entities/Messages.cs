using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities

{
    public class Messages
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SenderUsername { get; set; }

        [Required]
        public string ReceiverUsername { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}