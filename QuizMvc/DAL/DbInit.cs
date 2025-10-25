using QuizMvc.Models;

namespace QuizMvc.DAL
{
    public class DbInit
    {
        public static void Seed(QuizDbContext db)
        {
            // Optionally, you can seed some sample quizzes
            if (db.Quizzes.Any()) return; // Already seeded

            var sampleQuiz = new Quiz
            {
                Title = "Sample Quiz",
                Description = "This is a sample quiz to test the system.",
                DateCreated = DateTime.Now,
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "What is 2 + 2?",
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "3", IsCorrect = false, OptionLetter = "A" },
                            new Answer { Text = "4", IsCorrect = true, OptionLetter = "B" },
                            new Answer { Text = "5", IsCorrect = false, OptionLetter = "C" }
                        }
                    }
                }
            };

            db.Quizzes.Add(sampleQuiz);
            db.SaveChanges();
        }
    }
}
