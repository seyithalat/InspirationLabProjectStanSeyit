using System;

namespace InspirationLabProjectStanSeyit.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string FileAttachment { get; set; } // File path or name
    }
} 