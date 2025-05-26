using FileAnalysisService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FileAnalysisService.Data
{
    public class FileAnalysisDbContext : DbContext
    {
        public DbSet<AnalysisResult> AnalysisResults { get; set; }

        public FileAnalysisDbContext(DbContextOptions<FileAnalysisDbContext> options) : base(options) { }
    }
}