using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace ApiGateway.Clients
{
    public class FileAnalysisClient
    {
        private readonly HttpClient _httpClient;

        public FileAnalysisClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<object> AnalyzeFileAsync(Guid fileId)
        {
            var response = await _httpClient.PostAsync($"/analyze/{fileId}", null);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<object>(json);
        }
    }
}