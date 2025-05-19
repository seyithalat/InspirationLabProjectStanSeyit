using System;
using System.Collections.Generic;

namespace InspirationLabProjectStanSeyit
{
    public class AppData
    {
        public List<StudyLocation> StudyLocations { get; set; } = new List<StudyLocation>();
        public List<StudyNote> StudyNotes { get; set; } = new List<StudyNote>();
        public List<PlannerNote> PlannerNotes { get; set; } = new List<PlannerNote>();
        public UserSettings UserSettings { get; set; } = new UserSettings();
    }

    public class StudyLocation
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Population { get; set; }
    }

    public class StudyNote
    {
        public string FilePath { get; set; }
        public bool IsShared { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class PlannerNote
    {
        public DateTime Date { get; set; }
        public string Note { get; set; }
    }

    public class UserSettings
    {
        public string LastSelectedLocation { get; set; }
        public bool DarkMode { get; set; }
        public Dictionary<string, string> Preferences { get; set; } = new Dictionary<string, string>();
    }
} 