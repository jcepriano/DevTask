namespace DevTask.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string GitHubUsername { get; set; }
        public List<Task>? Tasks { get; set; } = new List<Task>();
        public List<GitHubRepository>? GitHubRepositories { get; set; } = new List<GitHubRepository>();
    }
}
