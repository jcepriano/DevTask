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

        /*
        public async Task<IActionResult> Index(string owner, string repoName)
        {
            var repo = new List<GitHubRepository>();
            repo = await _repositories.GetRepositories(owner, repoName);
            return View(repo);
        }
        */

        [Route("users/{id:int}/repos/new")]
        public IActionResult CreateRepo(int id)
        {
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
            var user = _context.Users
                .Where(u => u.Id == id)
                .Include(u => u.GitHubRepositories)
                .FirstOrDefault();

            var owner = Request.Cookies["CurrentUserIdUsername"];
            List<string> userInfo = new List<string>();
            userInfo.AddRange(owner.Split());

            var result = await _repositories.GetRepositories(userInfo[2], repo.Name);

            repo.OwnerName = user.FirstName;
            repo.User = user;
            repo.Description = result.Description;

            _context.GitHubRepositories.Add(repo);
            _context.SaveChanges();


            return Redirect($"/users/{user.Id}");

        }
    }
}
