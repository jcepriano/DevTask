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

            //try
            //{
                var response = await _httpClient.GetAsync(url);

                //if (response.IsSuccessStatusCode)
                //{
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<GitHubRepository>(stringResponse,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

                    //if (result.Id == 0 && result.Name is null && result.OwnerName is null)
                    //{
                    //    throw new BadHttpRequestException($"Invalid response from GitHub API. Check if {owner} and/or {repoName} are correct values.");
                    //}
                //}
                //else
                //{
                //    throw new HttpRequestException(response.ReasonPhrase);
                //}
            //}
            //catch
            //{
            //    throw new BadHttpRequestException($"Invalid response from GitHub API. Check if '{owner}' and/or '{repoName}' are correct values.");
            //}

            return result;
        }
    }
}
