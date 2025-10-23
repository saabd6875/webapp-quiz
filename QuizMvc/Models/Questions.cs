using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace QuizMvc.Models
{
    //represents one quiz question
    public class Question : IValidatableObject //validation logic
    {
        public int QuestionId { get; set; } //primary key for question

        [Required(ErrorMessage = "Please add a question")]
        public string Text { get; set; } = string.Empty; //question text
        public string? ImageUrl { get; set; }  //optional Url for image connected to question

        //answers connected to the question
        public List<Answer> Answers { get; set; } = new(); 

        public int QuizId { get; set; } //foreign key pointing to the quiz
        public Quiz Quiz { get; set; } = null!; //navigation property, quiz object


        //validation logic for question class
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)

        {   //various validation errors 
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
