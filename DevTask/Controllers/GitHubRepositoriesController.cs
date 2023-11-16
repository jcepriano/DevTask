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
            var repo = _context.GitHubRepositories
                .Include(r => r.User)
                .Include(r => r.Tasks)
                .ToList();
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
                .Include(u => u.User)
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

            if (result.Id == 0 && result.Name is null && result.OwnerName is null)
            {
                var errorView = new ErrorView
                {
                    ErrorMessage = $"Invalid response from GitHub API. Check if '{userInfo[2]}' and/or '{repo.Name}' are correct values."
                };
                return View("Error", errorView);
            }

            repo.OwnerName = user.FirstName + user.LastName;
            repo.User = user;
            repo.Id = result.Id;
            repo.Description = result.Description;
            
            if (repo.Description == null)
            {
                repo.Description = "No Description";
            }

            bool isDuplicate = _context.GitHubRepositories.Any(r => r.Id == repo.Id);

            if (isDuplicate)
            {
                // Return a user-friendly duplicate entry view
                var duplicateEntryViewModel = new ErrorView
                {
                    ErrorMessage = $"The repository '{repo.Name}' for user '{repo.OwnerName}' already exists in the database."
                };
                return View("DuplicateEntry", duplicateEntryViewModel);
            }

            _context.GitHubRepositories.Add(repo);
            _context.SaveChanges();


            return Redirect($"/users/{user.Id}");
        }
    }
}
