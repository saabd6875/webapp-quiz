using Microsoft.EntityFrameworkCore;

namespace QuizMvc.Models;

public class QuizDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answers> Answers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=quiz.db");
}