using DevTask.DataAccess;
using DevTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTask.Controllers
{
    public class TasksController : Controller
    {
        private readonly DevTaskContext _context;

        public TasksController(DevTaskContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("repos/{repoId:int}/tasks/new")]
        public IActionResult New(int repoId)
        {
            var repo = _context.GitHubRepositories
                .Where(r => r.Id == repoId)
                .Include(r => r.Tasks)
                .FirstOrDefault();

            return View(repo);
        }

        [HttpPost]
        [Route("repos/{repoId:int}/tasks")]
        public IActionResult Create(Models.Task task, int repoId)
        {
            var repo = _context.GitHubRepositories
                .Where(r => r.Id == repoId)
                .Include(r => r.User)
                .Include(r => r.Tasks)
                .FirstOrDefault();

            if (repo == null) return NotFound();

            task.IsActive = true;
            task.User = repo.User;

            task.GitHubRepository = repo;
            repo.Tasks.Add(task);
            _context.SaveChanges();

            return RedirectToAction("Index", "GitHubRepositories");
        }
    }
}
