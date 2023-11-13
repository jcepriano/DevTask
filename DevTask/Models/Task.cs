namespace DevTask.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public User User { get; set; }
        public GitHubRepository GitHubRepository { get; set; }
    }
}
