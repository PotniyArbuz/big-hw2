using FileStoringService.Data;
using FileStoringService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileStoringService.Controllers
{
    [ApiController]
    [Route("files")]
    public class FilesController : ControllerBase
    {
        private readonly FileStoringDbContext _dbContext;

        public FilesController(FileStoringDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using var reader = new StreamReader(file.OpenReadStream());
            var content = await reader.ReadToEndAsync();
            Console.WriteLine($"Raw file content: [{content ?? "null"}] (Length: {content?.Length ?? 0})");

            var fileEntity = new FileEntity
            {
                FileId = Guid.NewGuid(),
                UploadTimestamp = DateTime.UtcNow,
                Content = content
            };

            _dbContext.Files.Add(fileEntity);
            await _dbContext.SaveChangesAsync();

            return Ok(new { fileId = fileEntity.FileId });
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetFile(Guid fileId)
        {
            var fileEntity = await _dbContext.Files.FindAsync(fileId);
            if (fileEntity == null)
                return NotFound();

            return Ok(new
            {
                fileId = fileEntity.FileId,
                uploadTimestamp = fileEntity.UploadTimestamp,
                content = fileEntity.Content
            });
        }
    }
}