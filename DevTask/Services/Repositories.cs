using DevTask.Models;
using System.Text.Json;

namespace DevTask.Services
{
    public class Repositories
    {
        private readonly HttpClient _httpClient;

        public Repositories(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("GitHubApi");
        }

        public async Task<GitHubRepository> GetRepository()
        {
            var url = string.Format("/jcepriano/SceneSherpa");
            var result = new GitHubRepository();
            var response = await _httpClient.GetAsync(url);

            var stringResponse = await response.Content.ReadAsStringAsync();
            result = JsonSerializer.Deserialize<GitHubRepository>(stringResponse,
                new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            return result;
        }
    }
}
