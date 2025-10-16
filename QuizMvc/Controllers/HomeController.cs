using Microsoft.AspNetCore.Mvc;

namespace QuizMvc.Controllers
{

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MakeQuiz()
        {
            return RedirectToAction("CreateQuiz", "Quiz");
        }

        public IActionResult TakeQuiz()
        {
            return RedirectToAction("TakeQuiz", "Quiz");
        }
    }
}

