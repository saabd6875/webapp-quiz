using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace QuizMvc.Models
{
    public class Question : IValidatableObject
    {
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Please add a question")]
        public string Text { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } 


        public List<Answer> Answers { get; set; } = new();

        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        
        {
            if (Answers == null || Answers.Count == 0)
            {
                yield return new ValidationResult("The question must have at least 2 answers.",
                new[] { nameof(Answers) });
            }
            else
            {
                if (Answers.Count > 4)
                {
                    yield return new ValidationResult("The question has a maximum of 4 answers",
                    new[] { nameof(Answers) });
                }

                if (!Answers.Any(a => a.IsCorrect))
                {
                    yield return new ValidationResult("Each question must have at least one correct answer.",
                    new[] { nameof(Answers) });
                }
            }
        }
    }
}
