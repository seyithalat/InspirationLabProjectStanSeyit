using System;

namespace InspirationLabProjectStanSeyit.Models
{
    public class StudyMaterial
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public int? StudyGroupId { get; set; }
        
        // Navigation properties
        public virtual User CreatedBy { get; set; }
        public virtual StudyGroup StudyGroup { get; set; }
    }
} 