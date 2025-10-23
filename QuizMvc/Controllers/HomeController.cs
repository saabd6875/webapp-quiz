using Microsoft.AspNetCore.Mvc;

namespace QuizMvc.Controllers
{

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}

