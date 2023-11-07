using Microsoft.AspNetCore.Mvc;

namespace DevTask.Controllers
{
    public class TasksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
