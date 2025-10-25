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

        // List all quizzes (for ViewQuizzes.cshtml)
        public async Task<List<Quiz>> GetAllQuizzesAsync()
        {
            return await _db.Quizzes
               // .Include(q => q.Category)
                .ToListAsync();
        }

        // Get one quiz with questions (for ViewOneQuiz + PlayQuiz)
        public async Task<Quiz?> GetQuizByIdAsync(int id)
        {
            return await _db.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
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
