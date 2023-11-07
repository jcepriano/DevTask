﻿using DevTask.Models;
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

        public async Task<GitHubRepository> GetRepository()
        {
            var url = string.Format("repos/jcepriano/SceneSherpa");
            var result = new GitHubRepository();
            var response = await _httpClient.GetAsync(url);

            //if (response.IsSuccessStatusCode)
            //{
                var stringResponse = await response.Content.ReadAsStringAsync();
                result = JsonSerializer.Deserialize<GitHubRepository>(stringResponse,
                    new JsonSerializerOptions()
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
            //}
            //else
            //{
            //    throw new HttpRequestException(response.ReasonPhrase);
            //}


            return result;
        }
    }
}
