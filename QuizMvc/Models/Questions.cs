
using System.ComponentModel.DataAnnotations;

namespace QuizMvc.Models
{
    public class Question : IValidatableObject
    {
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Please add a question")]
        public string Text { get; set; } = string.Empty;

        public List<Answers> Answers { get; set; } = new List<Answers>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Answers == null || Answers.Count == 0)
            {
                yield return new ValidationResult("The question must have at least one correct answer",
                new[] { nameof(Answers) });
            }
            else
            {
                if (Answers.Count > 4)
                {
                    yield return new ValidationResult("The question has a maximum of 4 correct answers",
                    new[] { nameof(Answers) });
                }

                if (!Answers.Any(a => a.IsCorrect))
                {
                    yield return new ValidationResult("Minst ett alternativ må være riktig",
                    new[] { nameof(Answers) });
                }
            }
        }

        public int QuizId { get; set; }
    }
}
