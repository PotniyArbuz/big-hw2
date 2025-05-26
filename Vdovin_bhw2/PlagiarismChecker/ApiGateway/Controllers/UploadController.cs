using ApiGateway.Clients;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("upload")]
    public class UploadController : ControllerBase
    {
        private readonly FileStoringClient _fileStoringClient;
        private readonly FileAnalysisClient _fileAnalysisClient;

        public UploadController(FileStoringClient fileStoringClient, FileAnalysisClient fileAnalysisClient)
        {
            _fileStoringClient = fileStoringClient;
            _fileAnalysisClient = fileAnalysisClient;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var fileId = await _fileStoringClient.UploadFileAsync(file);
                var analysisResult = await _fileAnalysisClient.AnalyzeFileAsync(fileId);
                return Ok(analysisResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}