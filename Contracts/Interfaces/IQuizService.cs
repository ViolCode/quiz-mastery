using Contracts.DTOs;

namespace Contracts.Interfaces
{
    public interface IQuizService
    {
        void CreateQuiz(int teacherId, CreateQuizRequestDTO request);
    }
}
