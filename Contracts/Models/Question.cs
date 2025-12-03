using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string Text { get; set; }
        public string QuestionType { get; set; } // SingleChoice, MultipleChoice
        public int CorrectAnswerId { get; set; }

        public Quiz Quiz { get; set; }
        public Answer CorrectAnswer { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }

}
