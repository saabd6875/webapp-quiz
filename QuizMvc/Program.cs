
using Microsoft.EntityFrameworkCore;
using QuizMvc.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// EF Core setup (SQLite)
builder.Services.AddDbContext<QuizDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("QuizConnection")));

// Repository pattern
builder.Services.AddScoped<IQuizRepository, QuizRepository>();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuizDbContext>();
    db.Database.EnsureCreated();
    DbInit.Seed(db);
}

app.MapDefaultControllerRoute();
app.Run();

