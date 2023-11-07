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

        public async Task<IActionResult> Index()
        {
            var repo = new GitHubRepository();
            repo = await _repositories.GetRepository();
            return View(repo);
        }
    }
}
