using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Visibility { get; set; } // Public, Private, Link
        public DateTime CreatedAt { get; set; }

        public ICollection<Question> Questions { get; set; }
        public ICollection<TeacherAssignment> TeacherAssignments { get; set; }
    }
}
