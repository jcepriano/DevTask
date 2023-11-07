using DevTask.DataAccess;
using DevTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace DevTask.Controllers
{
    public class UsersController : Controller
    {
        private readonly DevTaskContext _context;

        public UsersController(DevTaskContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("users/{id:int}")]
        public IActionResult Show(int id)
        {
            var user = _context.Users
                .Where(u => u.Id == id)
                .Include(u => u.GitHubRepositories)
                .FirstOrDefault();

            return View(user);
        }

        [Route("/users/new")]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(User user)
        {
            bool usernames = _context.Users.Any(u => u.GitHubUsername == user.GitHubUsername);

            if (usernames)
            {
                ModelState.AddModelError("Username", "Username already exists");
                TempData["ErrorMessage"] = "Username already exists";
                return Redirect("/users/new");
            }
            _context.Users.Add(user);
            _context.SaveChanges();

            return Redirect($"/users/{user.Id}");
        }

        [Route("users/{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            return View(user);
        }

        [HttpPost]
        [Route("users/{id:int}")]
        public IActionResult Update(int id, User user)
        {
            //var existingUser = _context.Users.Find(id);
            //existingUser.Password = user.Password;
            user.Id = id;
            _context.Users.Update(user);
            _context.SaveChanges();

            return RedirectToAction("show", new { id = user.Id });
        }

        [HttpPost]
        [Route("users/{userId:int}/delete")]
        public IActionResult Delete(int userId)
        {
            var user = _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Tasks)
                .Include(u => u.GitHubRepositories)
                .FirstOrDefault(u => u.Id == userId);

            _context.Tasks.RemoveRange(user.Tasks);
            _context.GitHubRepositories.RemoveRange(user.GitHubRepositories);
            _context.Users.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("index");

        }

    }
}
