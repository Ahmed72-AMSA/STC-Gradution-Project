namespace STC.Models.Chat_Service
{
    public class Message
    {
        public int Id { get; set; }
        public string? Sender { get; set; } // Sender's username
        public string? Recipient { get; set; } // Recipient's username (for private messages)
        public string? Text { get; set; }
        public DateTime Timestamp { get; set; } // Store the time the message was sent

        // Optionally, a flag for whether the message has been read
        public bool IsRead { get; set; }
    }
}
