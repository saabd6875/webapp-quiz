using system;

namespace QuizMvc.Models

public class Answers {
    public int AnswerID { get; set; }
    public string Tekst { get; set; }
    public bool IsCorrect { get; set; }

    public int QuestionID { get; set; }
    public Question Question {get; set; }
}