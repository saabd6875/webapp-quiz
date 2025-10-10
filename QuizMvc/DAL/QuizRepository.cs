using Microsoft.EntityFrameworkCore;
using QuizMvc.Models;

namespace QuizMvc.DAL
{
    public class QuizRepository : IQuizRepository

    {
        private readonly QuizDbContext _db;

        public QuizRepository(QuizDbContext db)
        {
            _db = db;
        }

        public async Task<List<Quiz>> GetAllAsync()
        {
            return await _db.Quizzes.Include(q => q.Category).ToListAsync();
        }

        public async Task<Quiz?> GetByIdAsync(int id)
        {
            return await _db.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(a => a.Answers)
                .FirstOrDefaultAsync(q => q.QuizId == id);
        }

        public async Task AddAsync(Quiz quiz)
        {
            await _db.Quizzes.AddAsync(quiz);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}