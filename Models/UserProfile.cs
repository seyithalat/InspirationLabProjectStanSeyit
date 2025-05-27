using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspirationLabProjectStanSeyit.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Bio { get; set; }
        public DateTime? Birthday { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public string School { get; set; }
        public string Course { get; set; }
        public string Year { get; set; }
        public double StudyHours { get; set; }
    }
}
