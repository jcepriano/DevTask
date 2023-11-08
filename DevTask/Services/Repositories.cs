using DevTask.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DevTask.Services
{
    public class Repositories
    {
        private readonly HttpClient _httpClient;
        private readonly User _user;

        public Repositories(IHttpClientFactory clientFactory, User user)
        {
            _httpClient = clientFactory.CreateClient("GitHubApi");
            _httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("YourAppName", "1.0"));

            _user = user;
        }

        public async Task<List<GitHubRepository>> GetRepositories(string repoName)
        {
            string owner = _user.GitHubUsername;
            var url = $"repos/{owner}/{repoName}";
            var result = new List<GitHubRepository>();

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<List<GitHubRepository>>(stringResponse,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                }
                else
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch repository: " + ex.Message);
            }

            return result;
        }
    }
}
