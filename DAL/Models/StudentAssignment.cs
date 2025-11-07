using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class StudentAssignment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int AssignmentId { get; set; }
        public DateTime AssignedAt { get; set; }

        public ApplicationUser Student { get; set; }
        public TeacherAssignment Assignment { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }

}
