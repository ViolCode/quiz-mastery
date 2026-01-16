using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs
{
    public class StudentTakeAnswerDto
    {
        public int AnswerId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
