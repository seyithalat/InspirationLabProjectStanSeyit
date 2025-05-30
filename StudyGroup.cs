using System;
using System.Collections.Generic;

namespace InspirationLabProjectStanSeyit.Models
{
    public class StudyGroup
    {
        // Creation of StudyGroup class with properties to store study group details.
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        
        // Navigation properties
        public virtual User CreatedBy { get; set; }
        public virtual ICollection<User> Members { get; set; }
        public virtual ICollection<StudyMaterial> StudyMaterials { get; set; }
    }
} 