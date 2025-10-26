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
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        public async Task<IActionResult> Submit(TakeQuizViewModels model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid quiz submission for QuizId {QuizId}", model.QuizId);
                return View("TakeQuiz", model); // return back with validation messages
            }

            var quiz = await _repo.GetQuizByIdAsync(model.QuizId);
            if (quiz == null)
            {
                _logger.LogError("Quiz not found: {QuizId}", model.QuizId);
                return NotFound();
            }

            int score = 0;
            foreach (var q in model.Question)
            {
                var correctAnswer = quiz.Questions
                    .FirstOrDefault(quest => quest.QuestionId == q.Id)?
                    .Answers.FirstOrDefault(a => a.IsCorrect)?.OptionLetter;

                if (string.Equals(correctAnswer?.Trim(), q.SelectedAnswer?.Trim(), StringComparison.OrdinalIgnoreCase))
                    score++;
            }

            var resultVm = new SubmitQuizViewModel
            {
                QuizTitle = quiz.Title,
                Score = score,
                TotalQuestions = quiz.Questions.Count
            };

            return View("Submit", resultVm);
        }


         // ---------------- ACTIONS ON QUIZZES ----------------
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

        // GET: Show the edit form

        [HttpGet]
        public async Task<IActionResult> UpdateQuiz(int id)
        {
            var quiz = await _repo.GetQuizByIdAsync(id);
            if (quiz == null) return NotFound();

            var vm = new CreateQuizViewModels
            {
                QuizId = quiz.QuizId,
                Title = quiz.Title,
                Description = quiz.Description,
                Questions = quiz.Questions.Select(q => new CreateQuizViewModels.QuestionInput
                {
                    Text = q.Text,
                    OptionA = q.Answers.ElementAtOrDefault(0)?.Text ?? "",
                    OptionB = q.Answers.ElementAtOrDefault(1)?.Text ?? "",
                    OptionC = q.Answers.ElementAtOrDefault(2)?.Text ?? "",
                    OptionD = q.Answers.ElementAtOrDefault(3)?.Text ?? "",
                    CorrectOption = q.Answers.FirstOrDefault(a => a.IsCorrect)?.OptionLetter ?? ""
                }).ToList()
            };

            return View(vm);
        }

        // POST: Update quiz
        [HttpPost]
        public async Task<IActionResult> UpdateQuiz(CreateQuizViewModels model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid quiz update submission for QuizId {QuizId}", model.QuizId);
                return View(model);
            }

            var quiz = await _repo.GetQuizByIdAsync(model.QuizId);
            if (quiz == null)
            {
                _logger.LogError("Quiz not found with QuizId {QuizId}", model.QuizId);
                return NotFound();
            }

            // Update quiz metadata
            quiz.Title = model.Title;
            quiz.Description = model.Description;

            // Update existing questions
            for (int i = 0; i < model.Questions.Count; i++)
            {
                var submitted = model.Questions[i];

                // If question exists, update it; otherwise, add new
                Question dbQuestion;
                if (i < quiz.Questions.Count)
                {
                    dbQuestion = quiz.Questions[i];
                    dbQuestion.Text = submitted.Text;
                }
                else
                {
                    dbQuestion = new Question { Text = submitted.Text, Answers = new List<Answer>() };
                    quiz.Questions.Add(dbQuestion);
                }

                // Update answers
                var correctOption = (submitted.CorrectOption ?? "").Trim().ToUpper();
                var answers = new List<Answer>();

                if (!string.IsNullOrWhiteSpace(submitted.OptionA) || correctOption == "A")
                    answers.Add(new Answer { Text = submitted.OptionA ?? "", IsCorrect = correctOption == "A", OptionLetter = "A" });
                if (!string.IsNullOrWhiteSpace(submitted.OptionB) || correctOption == "B")
                    answers.Add(new Answer { Text = submitted.OptionB ?? "", IsCorrect = correctOption == "B", OptionLetter = "B" });
                if (!string.IsNullOrWhiteSpace(submitted.OptionC) || correctOption == "C")
                    answers.Add(new Answer { Text = submitted.OptionC ?? "", IsCorrect = correctOption == "C", OptionLetter = "C" });
                if (!string.IsNullOrWhiteSpace(submitted.OptionD) || correctOption == "D")
                    answers.Add(new Answer { Text = submitted.OptionD ?? "", IsCorrect = correctOption == "D", OptionLetter = "D" });

                // Replace answers in the DB question
                dbQuestion.Answers.Clear();
                dbQuestion.Answers.AddRange(answers);
            }

            try
            {
                await _repo.UpdateAsync(quiz);
                TempData["Message"] = "Quiz updated successfully!";
                return RedirectToAction("ViewOneQuiz", new { id = quiz.QuizId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quiz with QuizId {QuizId}", model.QuizId);
                ModelState.AddModelError("", "An error occurred while updating the quiz. Please try again.");
                return View(model);
            }
        }


        // POST: Delete quiz
        [HttpPost]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            try
            {
                await _repo.DeleteAsync(id);
                TempData["Message"] = "Quiz deleted successfully!";
                return RedirectToAction("ViewQuizzes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting quiz");
                TempData["Error"] = "An error occurred while deleting the quiz.";
                return RedirectToAction("ViewOneQuiz", new { id });
            }
        }


    }
                
}