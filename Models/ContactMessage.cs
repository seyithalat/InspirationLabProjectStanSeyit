using System;

namespace InspirationLabProjectStanSeyit.Models
{
    public class ContactMessage
    {
        //Creation of ContactMessage class with properties to store contact message details
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime SubmittedAt { get; set; }
        public int? UserId { get; set; }
        public bool Handled { get; set; }
        public int? HandledByAdminId { get; set; }
        public DateTime? HandledAt { get; set; }
    }
} 