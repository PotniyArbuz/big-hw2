using FileAnalysisService.Data;
using FileAnalysisService.Models;
using FileAnalysisService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileAnalysisService.Controllers
{
    [ApiController]
    [Route("analyze")]
    public class AnalysisController : ControllerBase
    {
        private readonly FileAnalysisDbContext _dbContext;
        private readonly FileStoringClient _fileStoringClient;

        public AnalysisController(FileAnalysisDbContext dbContext, FileStoringClient fileStoringClient)
        {
            _dbContext = dbContext;
            _fileStoringClient = fileStoringClient;
        }

        [HttpPost("{fileId}")]
        public async Task<IActionResult> AnalyzeFile(Guid fileId)
        {
            var fileData = await _fileStoringClient.GetFileAsync(fileId);
            if (fileData == null)
                return NotFound("File not found.");

            var content = fileData.Content;
            if (string.IsNullOrEmpty(content))
            {
                Console.WriteLine($"File content for ID {fileId} is null or empty.");
                return BadRequest("File content is empty or null.");
            }
            var uploadTimestamp = fileData.UploadTimestamp;

            var hash = ComputeHash(content);
            Console.WriteLine($"Computed hash for file {fileId}: {hash}");
            var (paragraphs, words, characters) = AnalyzeContent(content);

            var isPlagiarized = await _dbContext.AnalysisResults
                .AnyAsync(r => r.Hash == hash && r.UploadTimestamp < uploadTimestamp);
            Console.WriteLine(isPlagiarized
                ? $"Plagiarism detected for file {fileId}."
                : $"No plagiarism detected for file {fileId}.");

            var existingResult = await _dbContext.AnalysisResults
                .FirstOrDefaultAsync(r => r.FileId == fileId);

            AnalysisResult analysisResult;
            if (existingResult != null)
            {
                existingResult.Paragraphs = paragraphs;
                existingResult.Words = words;
                existingResult.Characters = characters;
                existingResult.IsPlagiarized = isPlagiarized;
                existingResult.Hash = hash;
                existingResult.UploadTimestamp = uploadTimestamp;
                analysisResult = existingResult;
                Console.WriteLine($"Updated existing analysis for file {fileId}.");
            }
            else
            {
                analysisResult = new AnalysisResult
                {
                    FileId = fileId,
                    UploadTimestamp = uploadTimestamp,
                    Paragraphs = paragraphs,
                    Words = words,
                    Characters = characters,
                    Hash = hash,
                    IsPlagiarized = isPlagiarized
                };
                _dbContext.AnalysisResults.Add(analysisResult);
                Console.WriteLine($"Created new analysis for file {fileId}.");
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error saving analysis for file {fileId}: {ex.InnerException?.Message}");
                return StatusCode(500, "Failed to save analysis results.");
            }

            return Ok(new
            {
                fileId = analysisResult.FileId,
                paragraphs = analysisResult.Paragraphs,
                words = analysisResult.Words,
                characters = analysisResult.Characters,
                isPlagiarized = analysisResult.IsPlagiarized
            });
        }

        private string ComputeHash(string content)
        {
            using var sha256 = SHA256.Create();
            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException(nameof(content), "Content cannot be null or empty.");
            var bytes = Encoding.UTF8.GetBytes(content);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        private (int paragraphs, int words, int characters) AnalyzeContent(string content)
        {
            if (string.IsNullOrEmpty(content))
                return (0, 0, 0);
            var paragraphs = content.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
            var words = content.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            var characters = content.Length;
            return (paragraphs, words, characters);
        }
    }
}