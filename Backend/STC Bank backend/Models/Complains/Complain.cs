using System.ComponentModel.DataAnnotations.Schema;

namespace STC.Models
{
    public class Complain
    {
        public int Id { get; set; }
        public string? Describtion { get; set; } 
        public string? Recipient { get; set; } 
        public bool Solved { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime? EndDate { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; } 

        public virtual SignUp? User { get; set; }
    }
}
