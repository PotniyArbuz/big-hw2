using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FileAnalysisService.Services
{
    public class FileStoringClient
    {
        private readonly HttpClient _httpClient;

        public FileStoringClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FileData> GetFileAsync(Guid fileId)
        {
            var response = await _httpClient.GetAsync($"/files/{fileId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FileData>(json);
        }
    }

    public class FileData
    {
        [JsonPropertyName("fileId")]
        public Guid FileId { get; set; }

        [JsonPropertyName("uploadTimestamp")]
        public DateTime UploadTimestamp { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}