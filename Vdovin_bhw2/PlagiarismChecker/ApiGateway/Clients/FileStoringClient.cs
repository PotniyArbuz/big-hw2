using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ApiGateway.Clients
{
    public class FileStoringClient
    {
        private readonly HttpClient _httpClient;

        public FileStoringClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Guid> UploadFileAsync(IFormFile file)
        {
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(file.OpenReadStream());
            content.Add(streamContent, "file", file.FileName);

            var response = await _httpClient.PostAsync("/files", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return Guid.Parse(result["fileId"]);
        }
    }
}