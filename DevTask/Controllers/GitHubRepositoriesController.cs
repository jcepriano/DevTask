using DevTask.DataAccess;
using DevTask.Models;
using DevTask.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DevTask.Controllers
{
    public class GitHubRepositoriesController : Controller
    {
        private readonly Repositories _repositories;
        private readonly DevTaskContext _context;

        public GitHubRepositoriesController(Repositories repositories, DevTaskContext context)
        {
            _repositories = repositories;
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
            var repo = _context.GitHubRepositories.ToList();
            return View(repo);
        }

        [Route("users/{userId:int}/repos/{repoId}")]
        public IActionResult Show(int userId, int repoId)
        {
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
            var repo = _context.GitHubRepositories
                .Where(u => u.User.Id == userId)
                .Where(u => u.Id == repoId)
                .Include(u => u.Tasks)
                .FirstOrDefault();

            return View(repo);
        }
        

        [Route("users/{id:int}/repos/new")]
        public IActionResult CreateRepo(int id)
        {
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
            var user = _context.Users
                .Where(u => u.Id == id)
                .Include(u => u.GitHubRepositories)
                .FirstOrDefault();

            return View(user);
        }

        [HttpPost]
        [Route("users/{id:int}/repos")]
        public async Task<IActionResult> Index(GitHubRepository repo, int id)
        {
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
            var user = _context.Users
                .Where(u => u.Id == id)
                .Include(u => u.GitHubRepositories)
                .FirstOrDefault();

            var owner = Request.Cookies["CurrentUser"];
            List<string> userInfo = new List<string>();
            userInfo.AddRange(owner.Split());

            var result = await _repositories.GetRepositories(userInfo[2], repo.Name);

            repo.OwnerName = user.FirstName + user.LastName;
            repo.User = user;
            repo.Id = result.Id;
            repo.Description = result.Description;

            _context.GitHubRepositories.Add(repo);
            _context.SaveChanges();


            return Redirect($"/users/{user.Id}");
        }
    }
}
