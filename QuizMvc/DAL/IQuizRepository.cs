using QuizMvc.Models;

namespace QuizMvc.DAL
{
    public interface IQuizRepository
    {
        Task<List<Quiz>> GetAllQuizzesAsync();   // for ViewQuizzes
        Task<Quiz?> GetQuizByIdAsync(int id); // for viewing details on one quiz
        Task AddAsync(Quiz quiz);
        Task SaveAsync();
    }
}