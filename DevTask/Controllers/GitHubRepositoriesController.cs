using DevTask.DataAccess;
using DevTask.Models;
using DevTask.Services;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult CreateRepo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(GitHubRepository repo, User user)
        {
            var owner = Request.Cookies["CurrentUserIdUsername"];
            List<string> userInfo = new List<string>();
            userInfo.AddRange(owner.Split());

            var result = await _repositories.GetRepositories(userInfo[2], repo.Name);

            _context.GitHubRepositories.Add(repo);
            _context.SaveChanges();


            return RedirectToAction("show", "UsersController");

        }
    }
}
