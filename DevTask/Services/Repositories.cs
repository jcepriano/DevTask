using DevTask.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DevTask.Services
{
    public class Repositories
    {
        private readonly HttpClient _httpClient;

        public Repositories(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("GitHubApi");
            _httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("YourAppName", "1.0"));

        }

        public async Task<GitHubRepository> GetRepositories(string owner, string repoName)
        {
            var url = $"repos/{owner}/{repoName}";
            var result = new GitHubRepository();

            var response = await _httpClient.GetAsync(url);

            var stringResponse = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<GitHubRepository>(stringResponse,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            return result;
        }
    }
}
