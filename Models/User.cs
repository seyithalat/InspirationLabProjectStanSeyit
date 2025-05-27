using System;
using System.Collections.Generic;

namespace InspirationLabProjectStanSeyit.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool Banned { get; set; }

        // Navigation properties
        public virtual ICollection<StudyGroup> StudyGroups { get; set; }
        public virtual ICollection<GameScore> GameScores { get; set; }
    }
} 