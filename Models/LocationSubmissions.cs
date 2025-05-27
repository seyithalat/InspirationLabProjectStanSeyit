using System;

namespace InspirationLabProjectStanSeyit.Models
{
    public class LocationSubmission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
} 