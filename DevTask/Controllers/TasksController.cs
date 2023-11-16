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
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
            return View();
        }

        [Route("repos/{repoId:int}/tasks/new")]
        public IActionResult New(int repoId)
        {
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
            var repo = _context.GitHubRepositories
                .Where(r => r.Id == repoId)
                .Include(r => r.Tasks)
                .Include(r => r.User)
                .FirstOrDefault();

            return View(repo);
        }

        [HttpPost]
        [Route("users/{userId:int}/repos/{repoId:int}/tasks")]
        public IActionResult Create(Models.Task task, int repoId, int userId)
        {
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
            var repo = _context.GitHubRepositories
                .Where(r => r.Id == repoId)
                .Include(r => r.User)
                .Include(r => r.Tasks)
                .FirstOrDefault();

            if (repo == null) return NotFound();
            if (task.Description == null) task.Description = "";
            
            task.IsActive = true;
            task.User = repo.User;

            task.GitHubRepository = repo;
            repo.Tasks.Add(task);
            _context.SaveChanges();

            return Redirect($"/users/{userId}/repos/{repoId}");
        }

        [Route("repos/{repoId:int}/tasks/{taskId:int}/edit")]
        public IActionResult Edit(int taskId)
        {
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
            var task = _context.Tasks
                .Where(t => t.Id == taskId)
                .Include(t => t.User)
                .Include(t => t.GitHubRepository)
                .FirstOrDefault();
            if (task.Description == null) task.Description = "";

            return View(task);
        }

        [HttpPost]
        [Route("users/{userId:int}/repos/{repoId:int}/tasks/{taskId:int}")]
        public IActionResult Update(Models.Task task, int taskId, int repoId, int userId)
        {
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
            task.Id = taskId;
            if (task == null) return NotFound();
            if (task.Description == null) task.Description = "";
            task.IsActive = true;
            //task.User = repo.User;

            //task.GitHubRepository = repo;
            _context.Tasks.Update(task);
            _context.SaveChanges();

            return Redirect($"/users/{userId}/repos/{repoId}");
        }
    }
}
