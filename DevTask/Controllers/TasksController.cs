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

        [Route("/users/{userId:int}/repos/{repoId:int}/tasks/new")]
        public IActionResult New(int userId, int repoId)
        {
            var repo = _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.GitHubRepositories)
                .ThenInclude(repo => repo.Tasks)
                .FirstOrDefault();

            return View(repo);
        }

        [HttpPost]
        [Route("/users/{userId:int}/repos/{repoId:int}/tasks")]
        public IActionResult Create(int userId)
        {
            var repo = _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.GitHubRepositories)
                .ThenInclude(repo => repo.Tasks)
                .FirstOrDefault();


        }
    }
}
