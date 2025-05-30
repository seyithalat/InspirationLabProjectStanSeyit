using System;

namespace InspirationLabProjectStanSeyit.Models
{
    public class GameScore
    {
        // Creation of GameScore class with properties to store game score details
        public int Id { get; set; }
        public int UserId { get; set; }
        public string GameType { get; set; }  // e.g., "MathGame", "TriviaGame", "WordScrambleGame"
        public int Score { get; set; }
        public DateTime PlayedAt { get; set; }
        
        // Navigation property
        public virtual User User { get; set; }
    }
} 