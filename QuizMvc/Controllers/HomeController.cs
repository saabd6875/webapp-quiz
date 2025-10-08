using Microsoft.AspNetCore.Mvc;

namespace QuizMvc.Controllers
{
    public class Homecontroller : Controller {
    public IActionResult Index()
    {
        return View();
        }
    }
}