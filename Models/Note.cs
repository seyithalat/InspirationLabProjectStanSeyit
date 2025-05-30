using System;
using System.Collections.Generic;

namespace InspirationLabProjectStanSeyit.Models
{
    public class Note
    {
        // Creation of Note class with properties to store note details
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public byte[] Content { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Sharing information
        public string SharedByUsername { get; set; }
        public string SharedWithUsername { get; set; }
        public DateTime? SharedAt { get; set; }
        public bool IsSender { get; set; }
    }
}
