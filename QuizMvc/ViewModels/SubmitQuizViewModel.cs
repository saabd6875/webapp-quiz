namespace QuizMvc.ViewModels
{

    public class SubmitQuizViewModel
    {
        public required string QuizTitle { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
    }
}