namespace DevTask.Models
{
    public class GitHubRepository
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string Description { get; set; }
        public List<Task>? Tasks { get; set; } = new List<Task>();
        public User User { get; set; }

    }
}
