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
            // Add any additional headers if required for authentication (e.g., access token)
            // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "your_access_token");
        }

        public async Task<GitHubRepository> GetRepository(string owner, string repoName)
        {
            var url = $"repos/{owner}/{repoName}";
            var result = new GitHubRepository();

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<GitHubRepository>(stringResponse,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                }
                else
                {
                    // If request fails, handle the error or throw an exception
                    // For example, you can throw an exception with the response reason phrase
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                // Log the error, return a default GitHubRepository object, or throw the exception
                throw new Exception("Failed to fetch repository: " + ex.Message);
            }

            return result;
        }
    }
}
