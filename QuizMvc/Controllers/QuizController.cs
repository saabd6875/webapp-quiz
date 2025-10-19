using Microsoft.AspNetCore.Mvc;
using QuizMvc.DAL;
using QuizMvc.Models;
using QuizMvc.ViewModels;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

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
        public IActionResult CreateQuiz()
        {
            // Returnerer tom ViewModel for visning
            var vm = new CreateQuizViewModels
            {
                Questions = new List<CreateQuizViewModels.QuestionInput>
                {
                    new CreateQuizViewModels.QuestionInput()
                }
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuiz(CreateQuizViewModels vm)
        {
            // --- VALIDERING ---
            if (string.IsNullOrWhiteSpace(vm.Title))
                ModelState.AddModelError("", "Quiz title is required.");

            if (vm.Questions.Count == 0)
                ModelState.AddModelError("", "A quiz must have at least one question.");

            if (vm.Questions.Count > 10)
                ModelState.AddModelError("", "A quiz cannot have more than 10 questions.");

            foreach (var q in vm.Questions)
            {
                if (string.IsNullOrWhiteSpace(q.Text))
                    ModelState.AddModelError("", "Each question must have text.");

                if (string.IsNullOrWhiteSpace(q.CorrectOption))
                    ModelState.AddModelError("", $"Question '{q.Text}' must have a correct option.");

                // Teller antall gyldige svar
                var answersCount = new[] { q.OptionA, q.OptionB, q.OptionC, q.OptionD }
                    .Count(opt => !string.IsNullOrWhiteSpace(opt));

                if (answersCount < 2 || answersCount > 4)
                    ModelState.AddModelError("", $"Question '{q.Text}' must have between 2 and 4 answers.");
            }

            if (!ModelState.IsValid)
                return View(vm);

            string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            foreach (var q in vm.Questions)
            {
                if(q.Image !=null && q.Image.Length >0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(q.Image.FileName);
                    string filePath = Path.Combine(uploadsPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await q.Image.CopyToAsync(stream);

                    }
                    q.ImageUrl  = "/uploads/" + fileName;
                }
            }

            // --- MAPPING ---
            var quiz = new Quiz
            {
                Title = vm.Title,
                Description = vm.Description,
                CategoryId = vm.CategoryId,
                Questions = vm.Questions.Select(q => new Question
                {
                    Text = q.Text,
                    ImageUrl= q.ImageUrl,
                    Answers = new List<Answer>
                    {
                        new Answer { Text = q.OptionA, IsCorrect = q.CorrectOption == "A" },
                        new Answer { Text = q.OptionB, IsCorrect = q.CorrectOption == "B" },
                        new Answer { Text = q.OptionC, IsCorrect = q.CorrectOption == "C" },
                        new Answer { Text = q.OptionD, IsCorrect = q.CorrectOption == "D" },
                    }
                    .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                    .ToList()
                }).ToList()
            };

            try
            {
                await _repo.AddAsync(quiz);
                await _repo.SaveAsync();

                TempData["Message"] = "Quiz successfully created!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quiz");
                ModelState.AddModelError("", "An error occurred while saving the quiz.");
                return View(vm);
            }
        }

        // ---------------- QUIZ TAKING MODE ----------------
        [HttpGet]
        public async Task<IActionResult> TakeQuiz(int id)
        {
            var quiz = await _repo.GetByIdAsync(id);
            if (quiz == null)
                return NotFound();

            // Mapper quiz til ViewModel
            var vm = new TakeQuizViewModels
            {
                QuizId = quiz.QuizId,
                Title = quiz.Title,
                Question = quiz.Questions.Select(q => new TakeQuizViewModels.QuestionItem
                {
                    Id = q.QuestionId,
                    Text = q.Text,
                    OptionA = q.Answers.ElementAtOrDefault(0)?.Text ?? "",
                    OptionB = q.Answers.ElementAtOrDefault(1)?.Text ?? "",
                    OptionC = q.Answers.ElementAtOrDefault(2)?.Text ?? "",
                    OptionD = q.Answers.ElementAtOrDefault(3)?.Text ?? ""
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int id, Dictionary<int, string> userAnswers)
        {
            var quiz = await _repo.GetByIdAsync(id);
            if (quiz == null)
                return NotFound();

            int score = 0;

            foreach (var question in quiz.Questions)
            {
                if (userAnswers.TryGetValue(question.QuestionId, out string? selectedOption))
                {
                    // Finn korrekt svartekst
                    var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect)?.Text;
                    if (selectedOption == correctAnswer)
                        score++;
                }
            }

            ViewBag.Score = score;
            ViewBag.Total = quiz.Questions.Count;
            ViewBag.QuizTitle = quiz.Title;

            return View("Result");
        }
    }
}
