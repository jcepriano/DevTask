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
        public async Task<IActionResult> Index(GitHubRepository repo)
        {
            var result = await _repositories.GetRepositories(repo.Name);

            if (result.Any())
            {
                return RedirectToAction("show", "UsersController");
            }
            else
            {
                return BadRequest("Error occurred while creating the repo");
            }
        }
    }
}
