using System.Security.Cryptography;
using System.Text;

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

        public string ReturnEncryptedString(string stringToEncrypt)
        {
            HashAlgorithm sha = SHA256.Create();
            byte[] firstInputBytes = Encoding.ASCII.GetBytes(stringToEncrypt);
            byte[] firstInputDigested = sha.ComputeHash(firstInputBytes);

            StringBuilder firstInputBuilder = new StringBuilder();
            foreach (byte b in firstInputDigested)
            {
                Console.Write(b + ", ");
                firstInputBuilder.Append(b.ToString("x2"));
            }
            return firstInputBuilder.ToString();
        }

    }
}
