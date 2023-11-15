using DevTask.DataAccess;
using DevTask.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
            return View();
        }

        [Route("users/{id:int}")]
        public IActionResult Show(int id)
        {
            var user = _context.Users
                .Where(u => u.Id == id)
                .Include(u => u.GitHubRepositories)
                .FirstOrDefault();

            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];

            return View(user);
        }

        [Route("/users/new")]
        public IActionResult New()
        {
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
            return View();
        }

        [HttpPost]
        public IActionResult Index(User user)
        {
            //bool usernames = _context.Users.Any(u => u.GitHubUsername == user.GitHubUsername);

            //if (usernames)
            //{
            //    ModelState.AddModelError("Username", "Username already exists");
            //    TempData["ErrorMessage"] = "Username already exists";
            //    return Redirect("/users/new");
            //}
            user.Password = user.ReturnEncryptedString(user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            Response.Cookies.Append("CurrentUser", $"{user.Id} {user.FirstName} {user.GitHubUsername}");

            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];

            return Redirect($"/users/{user.Id}");
        }

        [Route("users/{id:int}/edit")]
        public IActionResult Edit(int id)
        {
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
            var user = _context.Users.Find(id);
            return View(user);
        }

        [HttpPost]
        [Route("users/{id:int}")]
        public IActionResult Update(int id, User user)
        {
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
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

            Response.Cookies.Delete("CurrentUser");

            return RedirectToAction("index");

        }

        [Route("/users/login")]
        public IActionResult LoginForm()
        {
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
            ViewData["FailedLogin"] = TempData["FailedLogin"];
            return View();
        }

        [HttpPost]
        [Route("/users/login/attempt")]
        public IActionResult LoginAttempt(string email, string password)
        {
            ViewData["CurrentUser"] = Request.Cookies["CurrentUser"];
            Console.WriteLine($"Email: {email}, Password: {password}");

            string FailedLogin = "Either your password or username is incorrect, please try again.";
            if (ModelState.IsValid)
            {

                if (email != null && password != null)
                {
                    //Method FindUserByUsername
                    var LoginAttemptUser = FindUserbyEmail(email);

                    if (LoginAttemptUser != null)
                    {
                        //Method CheckHashedPassword
                        if (LoginAttemptUser.Password != null && CheckHashedPassword(password, LoginAttemptUser))
                        {
                            //Method AppendIdUsernameCookie
                            AppendIdUsernameCookie(LoginAttemptUser);
                            return Redirect($"/users/{LoginAttemptUser.Id}");
                        }
                        else
                        {
                            //Method FailedLoginTempData
                            TempData["FailedLogin"] = FailedLogin;
                        }

                    }
                    else
                    {
                        TempData["FailedLogin"] = FailedLogin;
                    }
                }
            }
            else if (email == null || password == null)
            {
                TempData["FailedLogin"] = FailedLogin;
            }
            else if (email == null && password == null)
            {
                TempData["FailedLogin"] = FailedLogin;
            }
            return Redirect("/users/login");
        }

        public async System.Threading.Tasks.Task GoogleLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(c => new
            {
                c.Issuer,
                c.OriginalIssuer,
                c.Type,
                c.Value
            });

            return Json(claims);
        }

        [Route("/users/{id:int}/Logout")]
        public IActionResult Logout(int id)
        {
            if (id != null)
            {
                Response.Cookies.Delete("CurrentUser");
            }

            return Redirect("/githubrepositories");
        }


        public User FindUserbyEmail(string email)
        {
            return _context.Users.Where(e => e.Email == email).FirstOrDefault();
        }

        public bool CheckHashedPassword(string password, User LoginAttemptUser)
        {
            Console.WriteLine($"Input Password: {password}");
            Console.WriteLine($"Stored Hashed Password: {LoginAttemptUser.Password}");

            return LoginAttemptUser.Password == LoginAttemptUser.ReturnEncryptedString(password);
        }

        public void AppendIdUsernameCookie(User user)
        {
            Response.Cookies.Append("CurrentUser", $"{user.Id} {user.FirstName} {user.GitHubUsername}");
        }
    }
}
