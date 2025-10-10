using QuizMvc.Models;

namespace QuizMvc.DAL
{
    public interface IQuizRepository
    {
        Task<List<Quiz>> GetAllAsync();
        Task<Quiz?> GetByIdAsync(int id);
        Task AddAsync(Quiz quiz);
        Task SaveAsync();
    }
}