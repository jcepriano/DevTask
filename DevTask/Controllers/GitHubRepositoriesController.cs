using DevTask.Models;
using DevTask.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevTask.Controllers
{
    public class GitHubRepositoriesController : Controller
    {
        private readonly Repositories _repositories;
        public GitHubRepositoriesController(Repositories repositories)
        {
            _repositories = repositories;
        }

        public async Task<IActionResult> Index(string owner, string repoName)
        {
            var repo = new List<GitHubRepository>();
            repo = await _repositories.GetRepositories(owner, repoName);
            return View(repo);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string owner, User user)
        {
            owner = user.GitHubUsername;
            return RedirectToAction("show", "UsersController");
        }
    }
}
