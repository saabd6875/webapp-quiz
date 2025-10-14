using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizMvc.DAL;
using QuizMvc.Models;

namespace QuizMvc.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizRepository _repo;
        private readonly ILogger<QuizController> _logger;

        public QuizController(IQuizRepository repo, ILogger<QuizController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // ---------------- CREATE QUIZ MODE ----------------
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Quiz { Questions = new List<Question> { new Question() } });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Quiz quiz)
        {
            if (quiz.Questions.Count > 10)
                ModelState.AddModelError("", "A quiz cannot have more than 10 questions.");

            foreach (var question in quiz.Questions)
            {
                if (question.Answers.Count < 2 || question.Answers.Count > 4)
                    ModelState.AddModelError("", $"Question '{question.Text}' must have between 2 and 4 answers.");
                if (!question.Answers.Any(a => a.IsCorrect))
                    ModelState.AddModelError("", $"Question '{question.Text}' must have at least one correct answer.");
            }

            if (!ModelState.IsValid)
                return View(quiz);

            await _repo.AddAsync(quiz);
            await _repo.SaveAsync();

            TempData["Message"] = "Quiz successfully created!";
            return RedirectToAction("Index", "Home");
        }

        // ---------------- QUIZ TAKING MODE ----------------
        [HttpGet]
        public async Task<IActionResult> Take(int id)
        {
            var quiz = await _repo.GetByIdAsync(id);
            if (quiz == null) return NotFound();
            return View(quiz);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int id, Dictionary<int, int> selectedAnswers)
        {
            var quiz = await _repo.GetByIdAsync(id);
            if (quiz == null) return NotFound();

            int score = 0;
            foreach (var q in quiz.Questions)
            {
                if (selectedAnswers.TryGetValue(q.QuestionId, out int selectedAnswerId))
                {
                    var answer = q.Answers.FirstOrDefault(a => a.AnswerId == selectedAnswerId);
                    if (answer?.IsCorrect == true)
                        score++;
                }
            }

            ViewBag.Score = score;
            ViewBag.Total = quiz.Questions.Count;
            return View("Result");
        }
    }
}
