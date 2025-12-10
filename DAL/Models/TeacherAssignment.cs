using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class TeacherAssignment
    {
        public int Id { get; set; }
        public string TeacherId { get; set; }
        public int QuizId { get; set; }
        public DateTime CreatedAt { get; set; }
        public User Teacher { get; set; }
        public Quiz Quiz { get; set; }
        public ICollection<StudentAssignment> StudentAssignments { get; set; }
    }
}
