using Microsoft.AspNetCore.Mvc;

namespace QuizMvc.Controllers
{
    public class Homecontroller : Controller {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult MakeQuiz()
        {
            return RedirectToAction("CreateQuize", "Quize");
        }
        public IActionResult TakeQuiz()
        {
            return RedirectToAction("TakeQuiz", "Quiz");
        }
        
    }
}