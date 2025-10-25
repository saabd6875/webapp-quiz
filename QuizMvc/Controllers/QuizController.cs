using Microsoft.AspNetCore.Mvc;
using QuizMvc.DAL;
using QuizMvc.Models;
using QuizMvc.ViewModels;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Collections.Generic;

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

        // ---------------- CREATE QUIZ MODE -----------------
        [HttpGet] 
        public IActionResult CreateQuiz()
        {
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
            // Validation
            if (string.IsNullOrWhiteSpace(vm.Title))
                ModelState.AddModelError(nameof(vm.Title), "Quiz title is required.");

            if (vm.Questions.Count == 0)
                ModelState.AddModelError("", "A quiz must have at least one question.");

            if (vm.Questions.Count > 10)
                ModelState.AddModelError("", "A quiz cannot have more than 10 questions.");

            foreach (var q in vm.Questions.Select((value, i) => new { value, i }))
            {
                if (string.IsNullOrWhiteSpace(q.value.Text))
                    ModelState.AddModelError($"Questions[{q.i}].Text", "Each question must have text.");

                if (string.IsNullOrWhiteSpace(q.value.CorrectOption))
                    ModelState.AddModelError($"Questions[{q.i}].CorrectOption", "Each question must have a correct option.");

                var answersCount = new[] { q.value.OptionA, q.value.OptionB, q.value.OptionC, q.value.OptionD }
                    .Count(opt => !string.IsNullOrWhiteSpace(opt));

                if (answersCount < 2 || answersCount > 4)
                    ModelState.AddModelError($"Questions[{q.i}]", "Each question must have between 2 and 4 answers.");
            }

            if (!ModelState.IsValid)
                return View(vm);

            // --- create Upload Folder
            string uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            // saving quiz image
            string? quizImageUrl = null;
            if (vm.Image != null && vm.Image.Length > 0)
            {
                string quizFileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.Image.FileName);
                string quizFilePath = Path.Combine(uploadsPath, quizFileName);
                using (var stream = new FileStream(quizFilePath, FileMode.Create))
                {
                    await vm.Image.CopyToAsync(stream);
                }
                quizImageUrl = "/uploads/" + quizFileName;
            }

            // saving questions image 
            foreach (var q in vm.Questions)
            {
                if (q.Image != null && q.Image.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(q.Image.FileName);
                    string filePath = Path.Combine(uploadsPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await q.Image.CopyToAsync(stream);
                    }
                    q.ImageUrl = "/uploads/" + fileName;
                }
            }

            // Mapping to modell 
            var quiz = new Quiz
            {
                Title = vm.Title,
                Description = vm.Description,
                //CategoryId = vm.CategoryId,
                ImageUrl = quizImageUrl,
                Questions = vm.Questions.Select(q => new Question
                {
                    Text = q.Text,
                    ImageUrl = q.ImageUrl,
                    Answers = new List<Answer>
                    {
                        new Answer { Text = q.OptionA, IsCorrect = q.CorrectOption == "A", OptionLetter = "A"  },
                        new Answer { Text = q.OptionB, IsCorrect = q.CorrectOption == "B", OptionLetter = "B"  },
                        new Answer { Text = q.OptionC, IsCorrect = q.CorrectOption == "C", OptionLetter = "C"  },
                        new Answer { Text = q.OptionD, IsCorrect = q.CorrectOption == "D", OptionLetter = "D"  },
                    }
                    .Where(a => !string.IsNullOrWhiteSpace(a.Text))
                    .ToList()
                }).ToList()
            };

            // Saving to database
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
            var quiz = await _repo.GetQuizByIdAsync(id);
            if (quiz == null)
                return NotFound();

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
            var quiz = await _repo.GetQuizByIdAsync(id);
            if (quiz == null)
                return NotFound();

            int score = 0;

            foreach (var question in quiz.Questions)
            {
                if (userAnswers.TryGetValue(question.QuestionId, out string? selectedOption))
                {

                    if (question.Answers.Any(a => a.IsCorrect && a.OptionLetter == selectedOption))
                    {
                        score++;
                    }
                }
            }

            ViewBag.Score = score;
            ViewBag.Total = quiz.Questions.Count;
            ViewBag.QuizTitle = quiz.Title;

            return View("Result");
        }
        public async Task<IActionResult> ViewQuizzes()
        {
            var quizzes = await _repo.GetAllQuizzesAsync();

            var vm = quizzes.Select(q => new ViewQuizzesViewModel
            {
                QuizId = q.QuizId,
                Title = q.Title,
                ImageUrl = q.ImageUrl
            }).ToList();

            return View(vm);
        }

        public async Task<IActionResult> ViewOneQuiz(int id)
        {
            var quiz = await _repo.GetQuizByIdAsync(id);
            if (quiz == null) return NotFound();

            var vm = new ViewOneQuizViewModel
            {
                QuizId = quiz.QuizId,
                Title = quiz.Title,
                Description = quiz.Description,
                ImageUrl = quiz.ImageUrl
            };

            return View(vm);
        }

    }
                
}