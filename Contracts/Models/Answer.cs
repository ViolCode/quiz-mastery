using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public int? StudentAssignmentId { get; set; }

        public Question Question { get; set; }
        public StudentAssignment StudentAssignment { get; set; }
    }

}
