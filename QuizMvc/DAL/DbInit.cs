using QuizMvc.Models;

namespace QuizMvc.DAL
{
    public class DbInit
    {
        public static void Seed(QuizDbContext db)
        {
            if (db.Categories.Any()) return; // Already seeded

            var categories = new List<Category>
            {
                new Category { Name = "General Knowledge" },
                new Category { Name = "Science" },
                new Category { Name = "History" },
                new Category { Name = "Retoro"},
                new Category { Name = "fun"}
            };

            db.Categories.AddRange(categories);
            db.SaveChanges();
        }
    }
}
